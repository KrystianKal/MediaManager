module MediaManager.Features.Media.RemoveTagsFromMedia

open System
open MediaManager.Features.Common
open MediaManager.Features.Common.Responses
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp

let Apply (id:Guid) (tags: string list) connectionString =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        WITH tags_to_use AS (
            SELECT t.id FROM tags t
            WHERE t.name = ANY(@tagNames::text[])
        )
        DELETE FROM media_tags mt
        WHERE mt.media_id = @mediaId AND mt.tag_id in tags_to_use
    "
    |> Sql.parameters [
                       "mediaId", Sql.uuid id
                       "tagNames", Sql.stringArray  (Array.ofList <| tags)
                       ]
    |> Sql.executeNonQueryAsync

type RemoveTagsRequest = {
    TagNames: string list
}
let RegisterEndpoint path (app: WebApplication) connectionString =
    app.MapDelete(path,
       EndpointDelegate<string,RemoveTagsRequest>(fun ctx id request->
            task{
                match Parsers.tryParseGuid id with
                | Some guid ->
                    let! _ = Apply guid request.TagNames connectionString
                    return! ctx.Ok ""
                | None -> 
                    return! ctx.BadRequest {| Error =  "Invalid Id format" |}
            }
       ))
        .WithName("Remove Tags from Media")
        .WithDisplayName("Remove Tags from Media")
        .WithOpenApi(fun o ->
            o.OperationId <- "RemoveTagsFromMedia"
            o.Summary <- "Remove Tags From Media"
            o )
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        |> ignore

