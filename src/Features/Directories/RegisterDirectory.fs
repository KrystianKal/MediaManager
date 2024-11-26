module MediaManager.Features.Directories.RegisterDirectory

open MediaManager.Database.Dto.DirectoryDto
open MediaManager.Features.Common
open MediaManager.Features.Directories
open MediaManager.Features.Common.Responses
open MediaManager.Models.Directory
open MediaManager.Models.Common
open MediaManager.Models
open MediaManager.RopResult
open MediaManager.Features.Processing
open MediaManager.Features.Media.AddMedia
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp
let private applyCommand connectionString (directory:Directory) =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        INSERT INTO directories VALUES
        (@id,@path,@fileCount,@size)
        ON CONFLICT DO NOTHING
    "
    |> Sql.parameters [
        "id", Sql.uuid directory.Id
        "path", Sql.text (Path.value directory.Path)
        "fileCount", Sql.int directory.MediaFileCount
        "size", Sql.int64 directory.Size
    ]
    |> Command.execute
    |>>= (fun _ -> succeed directory)
    
  
//TODO return created resource path
let private endpointHandler (connectionString:string) (fileTaskQueue) = EndpointDelegate<string>(fun path ->
    let scanMediaAndGenerateThumbnails = tapOnSuccessAsyncWithAsync( fun (dir:Directory) ->task{
        let! media = Metadata.scanFilesInDirectory dir.Path
        return media
        |> Thumbnail.generateThumbnails
        |> List.ofArray
        |> (addMedia connectionString dir.Id)
    } )
    
    let getDirectoryById (dir:Directory) =
        GetDirectoryById.query connectionString dir.Id

    let watchDirectory directory =
            Watcher.watchDirectory connectionString fileTaskQueue directory
        
    path
    |> Path.createAbsolute
    <!> (Directory.create)
    >>=| (applyCommand connectionString)
    |> scanMediaAndGenerateThumbnails
    |>>=| getDirectoryById
        |>>- watchDirectory
    <|!> created ""
    |> toHttpResult
)
  
/// Inserts directory to the database
/// and scans all media inside the directory
/// ,extracting their metadata
/// and finally generating their thumbnails
let RegisterPostEndpoint path (app:WebApplication) connectionString fileTaskQueue =
    app.MapPost(path, endpointHandler connectionString fileTaskQueue)
       .Produces<DirectoryDto>
    |> ignore
           // .WithName("Register directory to be tracked")
           // .WithDisplayName("Register directory")
           // .WithOpenApi(fun o ->
           //     o.OperationId <- "RegisterDir"
           //     o.Summary <- "Register directory for tracking"
           //     o )
           // .Produces(StatusCodes.Status201Created)
           // .Produces(StatusCodes.Status400BadRequest)
