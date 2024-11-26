module MediaManager.Features.Directories.ReactToOfflineChanges

open System
open System.Text.Json
open System.Text.Json.Serialization
open System.Threading.Channels
open MediaManager.Features.Directories.FileEvents
open MediaManager.Features.Directories.Watcher
open MediaManager.Features.Processing.Metadata
open MediaManager.Models
open MediaManager.Logger
open MediaManager.Models.Common.Path
open MediaManager.Models.Media
open Npgsql.FSharp

type MediaWithThumbnails =
    {
        [<JsonPropertyName "name">]
        Name: string
        [<JsonPropertyName "thumbnails">]
        Thumbnails: string list
    }
type private QueryResponse =
    {
        DirectoryId: Guid
        DirectoryPath: string
        Media: MediaWithThumbnails list
    }

type private FileComparison =
    {
        DirectoryPath: string
        DirectoryId: Guid
        DriveFiles: Media list
        DbFiles: MediaWithThumbnails list
    }
    
let private queryDirectoriesWithTheirFiles connectionString =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        SELECT
            d.id AS directory_id,
            d.path AS path,
            json_agg(
                json_build_object(
                'name', m.name || '.' || m.format,
                'thumbnails', m.thumbnails
                )
            ) as media
        FROM directories d
                 JOIN media m ON m.directory_id = d.id
        GROUP BY d.id
        "
    |> Sql.executeAsync (fun read ->
        {
            DirectoryId = read.uuid "directory_id"
            DirectoryPath = read.string "path"
            Media = ( read.text "media"
                    |> fun json ->
                        JsonSerializer.Deserialize<MediaWithThumbnails list>(json) )
        })
    
module private FileComparison =
    let private compareFiles (directoryPath: string) (dbMedia: MediaWithThumbnails) (fileMedia: Media) =
            let dbFullPath =directoryPath+dbMedia.Name
            let driveFullPath =fullName fileMedia
            String.Compare(dbFullPath,driveFullPath , StringComparison.InvariantCultureIgnoreCase ) = 0
    let findMissingFiles (comparison: FileComparison) =
        let compare = compareFiles comparison.DirectoryPath
        let existsOnDriveButNotInDb =
            comparison.DriveFiles
            |> List.filter (
                fun fileMedia ->
                not (
                        List.exists
                        <| (fun dbMedia -> compare dbMedia fileMedia )
                        <| comparison.DbFiles
                    )
                )
        let existsInDbButNotOnDrive =
            comparison.DbFiles
            |> List.filter (
                fun dbMedia ->
                not (
                        List.exists
                        <| (fun fileMedia -> compare dbMedia fileMedia )
                        <| comparison.DriveFiles
                    )
                )
        existsOnDriveButNotInDb, existsInDbButNotOnDrive
    

let private publishEvents
    (channel: Channel<FileEvent>)
    connectionString
    (directory:QueryResponse)
    (newFiles:Media list,deletedFiles: MediaWithThumbnails list)
    =
    task{
    log Information $"Found {newFiles.Length} new files."
    log Information $"Found {deletedFiles.Length} deleted files."
    for file in newFiles do
        do!
            FileCreatedEvent ((fullName file), directory.DirectoryId, connectionString)
            |> channel.Writer.WriteAsync
    for file in deletedFiles do
        do!
            FileDeletedEvent ((directory.DirectoryPath+file.Name), directory.DirectoryId, connectionString)
            |> channel.Writer.WriteAsync
   }
///For media that exist in db but not on drive
/// delete their thumbnails and remove them from db.
/// For media files that are not in db
/// add them and generate thumbs
let handleInconsistentFiles connectionString (channel: Channel<FileEvent>)= task{
    let! directoriesWithMedia = queryDirectoriesWithTheirFiles connectionString
    for directory in directoriesWithMedia do
        let! driveFiles =
            scanFilesInDirectory
            <| Path directory.DirectoryPath
            
        let comparision = {
            DirectoryPath = directory.DirectoryPath
            DirectoryId = directory.DirectoryId
            DriveFiles = driveFiles |> List.ofArray
            DbFiles = directory.Media 
        }
        do! FileComparison.findMissingFiles comparision
            |> publishEvents channel connectionString directory
        
    }
    