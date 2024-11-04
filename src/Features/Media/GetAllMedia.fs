module MediaManager.Features.Media.GetAllMedia

open System
open System.Threading.Tasks

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp
open MediaManager.Database.Dto.MediaDto
open MediaManager.Database.Serializers
open MediaManager.Features.Common.Responses

//TODO add pagination
//TODO add filtering
//TODO add sorting
let private Query connectionString: Task<MediaDto list> =
    connectionString
    |> Sql.connect
    //use coalesce instead of array_agg to get the json type
    |> Sql.query @"
        SELECT m.*,
            d.path as directory_path,
            COALESCE (json_agg(t), '[]'::json) as tags
        FROM media m
            INNER JOIN directories d ON m.directory_id = d.id
            LEFT JOIN media_tags mt ON m.id = mt.media_id
            LEFT JOIN tags t ON t.id = mt.tag_id
        GROUP BY m.id, d.id
        "
    |> Sql.executeAsync deserializeMedia
    
let RegisterEndpoint path (app: WebApplication) connectionString =
    app.MapGet(path, Func<HttpContext, Task> (fun (ctx: HttpContext) ->
        task {
            let! media = Query connectionString
            return! ctx.Ok media
        }) ) |>ignore