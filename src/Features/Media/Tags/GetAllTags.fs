module MediaManager.Features.Media.Tags.GetAllTags

open MediaManager.Features.Common.Responses
open Microsoft.AspNetCore.Builder
open Npgsql.FSharp
open MediaManager.Database.Serializers
let Query connectionString =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        SELECT * FROM tags 
    "
    |> Sql.executeAsync deserializeTag

let RegisterEndpoint path (app:WebApplication) connectionString =
    app.MapGet(path, EndpointDelegate( fun ctx ->
        task {
            let! tags = Query connectionString
            return! ctx.Ok tags
        }
    )) |> ignore