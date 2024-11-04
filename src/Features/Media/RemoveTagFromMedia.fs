module MediaManager.Features.Media.RemoveTagFromMedia

open MediaManager.Features.Common
open MediaManager.Features.Common.Responses
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp

let Apply id tag connectionString =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        WITH tagId AS (
            SELECT t.id FROM tags t
            WHERE t.name ILIKE @tag 
            LIMIT 1
        )
        DELETE FROM media_tags mt
        WHERE mt.media_id = @mediaId AND mt.tag_id = (SELECT id FROM tagId)
    "
    |> Sql.parameters [
                       "mediaId", Sql.uuid id
                       "tag", Sql.string tag
                       ]
    |> Sql.executeNonQueryAsync

let RegisterEndpoint path (app: WebApplication) connectionString =
    app.MapDelete(path,
       EndpointDelegate<string,string>(fun ctx id tag->
            task{
                match Parsers.tryParseGuid id with
                | Some guid ->
                    match! Apply guid tag connectionString with
                        | 1 -> return! ctx.Ok ""
                        | _ -> return! ctx.BadRequest {| Error = "Media does not have this tag" |}
                | None -> 
                    return! ctx.BadRequest {| Error =  "Invalid Id format" |}
            }
       ))
       .WithName("Remove Tag from Media")
       .WithDisplayName("Remove Tag from Media")
       .WithOpenApi(fun o ->
           o.OperationId <- "RemoveTagFromMedia"
           o.Summary <- "Remove Tag From Media"
           o )
       .Produces(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status400BadRequest)
       |> ignore


