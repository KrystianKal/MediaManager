module MediaManager.Features.Media.GetMediaById

open System
open System.Threading.Tasks
open MediaManager.Features.Common
open MediaManager.Features.Media.DAL
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp
open MediaManager.Database.Serializers
open MediaManager.Database.Dto.MediaDto
open MediaManager.Features.Common.Query
open MediaManager.Features.Common.Responses

let Query (id:Guid) connectionString: Task<Option<MediaDto>> =
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
    |> trySingleAsync
    
let RegisterEndpoint path (app: WebApplication) connectionString =
    app.MapGet(path,
       EndpointDelegate<string>(fun (ctx: HttpContext) id ->
            task{
                match Parsers.tryParseGuid id with
                | Some guid ->
                    let! media = Query guid connectionString
                    let! _ = UpdateViewCount.Apply guid connectionString
                    return! ctx.Ok media
                | None -> 
                    return! ctx.NotFound {| Error =  "Not found" |}
            }
       )) |> ignore
