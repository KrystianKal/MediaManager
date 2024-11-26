module MediaManager.Features.Media.UpdateViewCount

open System
open MediaManager.Features.Common
open Npgsql.FSharp

let applyCommand connectionString  (id: Guid) =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        UPDATE media
            SET view_count = view_count + 1
        WHERE id = @id 
    "
    |> Sql.parameters ["id", Sql.uuid id]
    |> Command.execute
     

