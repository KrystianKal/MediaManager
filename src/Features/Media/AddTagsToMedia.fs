module MediaManager.Features.Media.AddTagsToMedia

open System
open MediaManager.Features.Common
open MediaManager.Features.Common.Responses
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp

let apply (id:Guid) (tags: string list) connectionString =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        WITH tags_to_use AS (
            INSERT INTO tags (name)
                SELECT unnest(@tagNames::text[])
            ON CONFLICT (name) DO NOTHING
            RETURNING id  
        )
        INSERT INTO media_tags (media_id,tag_id)
            SELECT @mediaId, t.id
            FROM tags_to_use t
        ON CONFLICT (media_id, tag_id) DO NOTHING
    "
    |> Sql.parameters [
                       "mediaId", Sql.uuid id
                       "tagNames", Sql.stringArray  (Array.ofList <| tags)
                       ]
    |> Sql.executeNonQueryAsync

type AddTagsRequest = {
    TagNames: string list
}
let RegisterEndpoint path (app: WebApplication) connectionString =
    app.MapPost(path,
       EndpointDelegate<string,AddTagsRequest>(fun ctx id request->
            task{
                match Parsers.tryParseGuid id with
                | Some guid ->
                    let! _ = apply guid request.TagNames connectionString
                    return! ctx.Ok ""
                | None -> 
                    return! ctx.BadRequest {| Error =  "Invalid Id format" |}
            }
       ))
       .WithName("Add Tags to Media")
       .WithDisplayName("Add Tags to Media")
       .WithOpenApi(fun o ->
           o.OperationId <- "AddTagsToMedia"
           o.Summary <- "Add Tags to Media"
           o )
       .Produces(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status400BadRequest)
       |> ignore
