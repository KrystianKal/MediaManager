module MediaManager.Features.Media.RemoveMedia

open System
open System.IO
open System.Threading.Tasks
open MediaManager.Database.Dto
open MediaManager.Database.Dto.MediaDto
open MediaManager.RopResult
open Npgsql.FSharp
open MediaManager.Database.Serializers
open MediaManager.Features.Common.Query
open MediaManager.Features.Common.Command

let private applyCommand connectionString directoryId fileName ext
                            : Task<string array> =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        DELETE FROM media m
        WHERE
            m.name like @fileName
            AND m.format like @ext
            AND m.directory_id = @directoryId
        RETURNING *;
    "
    |> Sql.parameters [
        "fileName", Sql.string fileName
        "ext", Sql.string ext
        "directoryId", Sql.uuid directoryId
    ]
    |> Sql.executeRowAsync (fun read -> read.stringArray "thumbnails" )


//remove media from DB and its thumbnails from disc
let private deleteThumbnails (thumbnails: string array) =
        thumbnails |> Array.iter File.Delete
let removeMedia connectionString directoryId (fullPath:string) =
    task{
    let! thumbnails =
        applyCommand connectionString directoryId 
        <| Path.GetFileNameWithoutExtension fullPath
        <| (Path.GetExtension fullPath).TrimStart('.')
    deleteThumbnails thumbnails
    }
    