module MediaManager.Features.Media.Tags.CreateTag

open System
open MediaManager.Features.Common
open Npgsql.FSharp
//TODO validate
let Apply name connectionString  =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        INSERT INTO tags (name)
        VALUES (@name)
    "
    |> Sql.parameters ["name", Sql.text name]
    |> Command.execute
    

