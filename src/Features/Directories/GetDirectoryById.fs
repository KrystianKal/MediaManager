module MediaManager.Features.Directories.GetDirectoryById


open System
open MediaManager
open MediaManager.Database.Dto.DirectoryDto
open MediaManager.Features.Common.Responses
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp
open MediaManager.Models.Common.Id
open MediaManager.RopResult
open MediaManager.Database.Serializers
let query connectionString (id:Guid) =
    connectionString
    |> Sql.connect
    |> Sql.query @"
    SELECT * FROM directories WHERE id = @id;
    "
    |> Sql.parameters ["@id",Sql.uuid id]
    |> Sql.executeRowAsync deserializeDirectory
let private endpointHandler (connectionString:string) = EndpointDelegate<string>(fun id ->
    succeed id
    >>= createId
    >>=| (query connectionString)
    |>- Logger.logFailure Logger.Error "GetDirectoryById query failed"
    <|!> ok
    |> toHttpResult
)
let RegisterGetEndpoint path (app: WebApplication) connectionString =
    app.MapGet(path, endpointHandler connectionString )
        .Produces<DirectoryDto>
    |> ignore
