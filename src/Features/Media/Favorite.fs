module MediaManager.Features.Media.Favorite

open System
open MediaManager.Features.Common
open MediaManager.RopResult
open Microsoft.AspNetCore.Builder
open Npgsql.FSharp
open MediaManager.Features.Common.Responses
open MediaManager.Models.Common.Id

let private applyCommand connectionString isFavorite (id: Guid)=
    connectionString
    |> Sql.connect
    |> Sql.query @"
        UPDATE media
        SET is_favorite = @isFavorite 
        WHERE id = @id
    "
    |> Sql.parameters ["id", Sql.uuid id
                       "isFavorite", Sql.bool isFavorite]
    |> Command.execute
    |>>= (fun _ -> succeed id)
    
let private endpointHandler (connectionString: string) (isFavorite:bool) = EndpointDelegate<string>(fun id ->
    succeed id
    >>= createId
    >>=| applyCommand connectionString isFavorite
    |>>=| GetMediaById.query connectionString
    <|!> ok
    |> toHttpResult
)
let RegisterPostEndpoint path (app: WebApplication) connectionString =
    app.MapPost(path, endpointHandler connectionString true)
    |> ignore
let RegisterDeleteEndpoint path (app: WebApplication) connectionString =
    app.MapDelete(path, endpointHandler connectionString false)
    |> ignore
