module MediaManager.Features.Media.RemoveTagFromMedia

open MediaManager.Features.Common.Responses
open MediaManager.Database.Serializers
open MediaManager.Models.Common.Id
open MediaManager.Features.Common
open MediaManager.RopResult
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp

let applyCommand connectionString tag id  =
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
    |> Command.execute
    |>>= (fun _ ->succeed id)

//todo validate tag
let endpointHandler (connectionString: string) = EndpointDelegate<string,string>(fun id tag ->
    succeed id
    >>= createId
    >>=| (applyCommand connectionString tag)
    |>>=| (GetMediaById.query connectionString)
    <|!> ok
    |> toHttpResult
    )
let RegisterDeleteEndpoint path (app: WebApplication) connectionString =
    app.MapDelete(path,endpointHandler connectionString )
       .WithName("Remove Tag from Media")
       .WithDisplayName("Remove Tag from Media")
       .WithOpenApi(fun o ->
           o.OperationId <- "RemoveTagFromMedia"
           o.Summary <- "Remove Tag From Media"
           o )
       .Produces(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status400BadRequest)
       |> ignore


