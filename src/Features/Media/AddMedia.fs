module MediaManager.Features.Media.AddMedia

open System
open MediaManager.Database.Dto.MediaDto
open MediaManager.RopResult
open MediaManager.Messages
open MediaManager.Database.Serializers
open MediaManager.Features.Media.DAL.MediaMappers
open Npgsql.FSharp
open MediaManager.Models

let private applyCommand connectionString (m:Media list) (directoryId:Guid) =
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
let addMedia connectionString (directoryId: Guid) (media :Media list) =
      task{
          let! insertedRows = applyCommand connectionString media directoryId
          match insertedRows with
          | [] -> return fail NoRowsAffected
          | _ -> return succeed media
      }
