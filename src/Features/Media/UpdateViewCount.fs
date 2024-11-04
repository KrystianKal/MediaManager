module MediaManager.Features.Media.UpdateViewCount

open System
open Npgsql.FSharp

let Apply (id: Guid) connectionString  =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        UPDATE media
            SET view_count = view_count + 1
        WHERE id = @id 
    "
    |> Sql.parameters ["id", Sql.uuid id]
    |> Sql.executeNonQueryAsync
     

