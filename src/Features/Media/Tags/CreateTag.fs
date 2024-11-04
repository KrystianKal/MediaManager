module MediaManager.Features.Media.Tags.CreateTag

open System
open Npgsql.FSharp

//TODO Create a trigger that will delete unused tags older than 1 day
//May be not needed in case postgres is used UPSERT
let Apply name connectionString  =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        INSERT INTO tags (name)
        VALUES (@name)
    "
    |> Sql.parameters ["name", Sql.text name]
    |> Sql.executeNonQueryAsync
    

