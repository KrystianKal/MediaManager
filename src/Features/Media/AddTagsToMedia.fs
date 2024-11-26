module MediaManager.Features.Media.AddTagsToMedia

open System
open MediaManager.Features.Common
open MediaManager.Features.Common.Responses
open MediaManager.Models
open Microsoft.AspNetCore.Builder
open MediaManager.RopResult
open MediaManager.Models.Common.Id
open Npgsql.FSharp
open MediaManager.Features.Media

let private applyCommand connectionString (tags: string list) (id:Guid) =
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
    |> Command.execute
    |>>= (fun _ -> succeed id)

type private AddTagsRequest = {
    TagNames: string list
}
let private endpointHandler (connectionString:string) = EndpointDelegate<string,AddTagsRequest>( fun id request ->
    succeed id
    >>= createId
    >>=| (applyCommand connectionString request.TagNames)
    |>>=| (GetMediaById.query connectionString)
    <|!> ok
    |> toHttpResult
)
    
let RegisterPostEndpoint path (app: WebApplication) connectionString =
    app.MapPost(path, endpointHandler connectionString)
    |> ignore
