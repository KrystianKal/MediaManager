module MediaManager.Features.Directories.GetAllDirectories

open MediaManager
open MediaManager.Database.Dto.DirectoryDto
open MediaManager.Features.Common.Responses
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp
open MediaManager.RopResult
open MediaManager.Database.Serializers

let query connectionString =
    connectionString
    |> Sql.connect
    |> Sql.query " SELECT * FROM directories; "
    |> Sql.executeAsync deserializeDirectory
    
let private endpointHandler (connectionString:string) = EndpointDelegate(fun _ ->
    (query connectionString)
    |> aggregateAsync
    |>>- Logger.logSuccess Logger.Information "GetAllDirectories fetched" (fun dirs -> dirs.Length)
    |>- Logger.logFailure Logger.Error "GetAllDirectories query failed"
    <|!> ok
    |> toHttpResult
)
let RegisterGetEndpoint path (app: WebApplication) connectionString =
    app.MapGet(path, endpointHandler connectionString )
        .Produces<DirectoryDto array>
    |> ignore
