module MediaManager.Features.Directories.RegisterDirectory

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.OpenApi
open MediaManager.Features.Common.Responses
open MediaManager.Models.Directory
open MediaManager.Database.Dto.MediaDto
open MediaManager.Models.Common
open MediaManager.Features.Processing
open MediaManager.Models
open MediaManager.Features.Media.DAL.MediaMappers
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.OpenApi.Models
open Npgsql.FSharp
let apply (directory:Directory) connectionString =
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
    |> Sql.executeNonQueryAsync
let insertMedia (m:Media list) (directoryId:Guid) connectionString =
    let dtos = List.map mediaToDto m
    connectionString
    |> Sql.connect
    |> Sql.executeTransactionAsync
        [
           "INSERT INTO media(
               id, media_type, directory_id,
               name, width, height,
               file_size, format, thumbnails,
               duration_seconds, bit_rate )
           VALUES
           (
               @media_id, @type::media_type, @directory_id,
               @name, @width, @height,
               @file_size, @format, @thumbnails,
               @duration, @bit_rate
           )",
           dtos |> List.map ( fun dto ->
               [
               "@media_id", Sql.uuid dto.Id; "@type", Sql.string dto.Type;
               "@directory_id", Sql.uuid directoryId; "@name", Sql.string dto.Name;
               "@width", Sql.int dto.Width; "@height", Sql.int dto.Height;
               "@file_size", Sql.int64 dto.FileSize; "@format", Sql.string dto.Format;
               "@thumbnails", Sql.stringArray (dto.Thumbnails |> Set.toArray);
               "@duration", Sql.intOrNone dto.DurationSeconds;
               "@bit_rate", Sql.intOrNone dto.BitRate
               ] 
           ) 
        ]
    
    
//TODO return resource path as POST should
let private EndpointHandler connectionString =
  EndpointDelegate<string>(fun ctx path ->
  task {
      match Path.createAbsolute path with
      | Ok p ->
          let directory = Directory.create p
          let! didCreateRow = apply directory connectionString
          if didCreateRow = 1 then 
              let mediaList =
                  Metadata.ScanFilesInDirectory p
                  |> Thumbnail.generateThumbnails
                  |> List.ofArray
              let! _ = insertMedia mediaList directory.Id connectionString
              return! ctx.Created ""
          else
              return! ctx.BadRequest {| Error = "Directory is already tracked!" |}
      | Error e  ->
          return! ctx.BadRequest {| Error = e |}
  })
  
let RegisterEndpoint path (app:WebApplication) connectionString =
    app.MapPost(path, EndpointHandler connectionString )
       .WithName("Register directory to be tracked")
       .WithDisplayName("Register directory")
       .WithOpenApi(fun o ->
           o.OperationId <- "RegisterDir"
           o.Summary <- "Register directory for tracking"
           o )
       .Produces(StatusCodes.Status201Created)
       .Produces(StatusCodes.Status400BadRequest)
       |> ignore
