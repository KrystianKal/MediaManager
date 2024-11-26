module MediaManager.Features.Media.GetAllMedia

open MediaManager
open MediaManager.Features.Media.Responses
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp
open MediaManager.Database.Serializers
open MediaManager.Features.Common.Responses
open MediaManager.RopResult

//TODO add pagination
//TODO add filtering
//TODO add sorting
let private query connectionString=
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
    
    
let RegisterGetEndpoint path (app: WebApplication) connectionString =
    let mapDtosToResponses dtos =
        dtos |> List.map MediaResponse.fromDto
        
    app.MapGet(path, EndpointDelegate (fun _ ->
            query connectionString
            |> aggregateAsync
            |>>- Logger.logSuccess Logger.Information "GetAllMedia fetched" (fun m -> m.Length)
            <|!> mapDtosToResponses
            <|!> ok
            |> toHttpResult
        ) )
        .Produces<MediaResponse.MediaResponse array> 
    |>ignore