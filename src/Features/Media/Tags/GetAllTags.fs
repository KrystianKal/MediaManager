module MediaManager.Features.Media.Tags.GetAllTags

open MediaManager.Features.Common.Responses
open Microsoft.AspNetCore.Builder
open Npgsql.FSharp
open MediaManager.Database.Serializers
let query connectionString =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        SELECT * FROM tags 
    "
    |> Sql.executeAsync deserializeTag

let RegisterGetEndpoint path (app:WebApplication) (connectionString: string) =
    app.MapGet(path, EndpointDelegate( fun _ ->
       task{
           let! response = query connectionString
           return ok response
       }
    )) |> ignore