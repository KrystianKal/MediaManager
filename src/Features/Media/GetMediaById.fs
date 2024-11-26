module MediaManager.Features.Media.GetMediaById

open System
open System.Threading.Tasks
open MediaManager.Features.Media.Responses.MediaResponse
open MediaManager.Models.Common.Id
open MediaManager.RopResult
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp
open MediaManager.Features.Media
open MediaManager.Database.Serializers
open MediaManager.Database.Dto.MediaDto
open MediaManager.Features.Common.Query
open MediaManager.Features.Common.Responses

let query connectionString (id:Guid): Task<RopResult<MediaDto>> =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        SELECT m.*,d.path as directory_path, COALESCE (json_agg(t), '[]'::json) as tags
        FROM media m
            INNER JOIN directories d ON m.directory_id = d.id
            LEFT JOIN media_tags mt ON m.id = mt.media_id
            LEFT JOIN tags t ON t.id = mt.tag_id
        WHERE m.id = @id
        GROUP BY m.id, d.id
    "
    |> Sql.parameters ["id", Sql.uuid id]
    |> Sql.executeAsync deserializeMedia
    |> trySingleRAsync
let private endpointHandler (connectionString:string) = EndpointDelegate<string>(fun id ->
    let updateViewCount = (fun dto -> UpdateViewCount.applyCommand connectionString dto.Id)
    succeed id
    >>= createId
    >>=| (query connectionString)
    |>>-| updateViewCount
    <|!> fromDto
    <|!> ok
    |> toHttpResult
)
let RegisterGetEndpoint path (app: WebApplication) connectionString =
    app.MapGet(path, endpointHandler connectionString )
        .Produces<MediaResponse>
    |> ignore

