module MediaManager.Features.Media.Favorite

open System
open System.Threading.Tasks
open MediaManager.Features.Common
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Npgsql.FSharp
open MediaManager.Features.Common.Responses

let Apply (id: Guid) isFavorite connectionString  =
    connectionString
    |> Sql.connect
    |> Sql.query @"
        UPDATE media
            SET is_favorite = @isFavorite 
        WHERE id = @id 
    "
    |> Sql.parameters ["id", Sql.uuid id
                       "isFavorite", Sql.bool isFavorite]
    |> Sql.executeNonQueryAsync
    
let RegisterEndpoint path (app: WebApplication) connectionString =
    app.MapPost(path,
       Func<HttpContext, string,Task>( fun (ctx: HttpContext) id ->
            task{
                match Parsers.tryParseGuid id with
                | Some guid ->
                    let! _ = Apply guid true connectionString
                    return! ctx.Ok ""
                | None -> 
                    return! ctx.NotFound {| Error =  "Not found" |}
            }
       )) |> ignore
    app.MapDelete(path,
       Func<HttpContext, string,Task>( fun (ctx: HttpContext) id ->
            task{
                match Parsers.tryParseGuid id with
                | Some guid ->
                    let! _ = Apply guid false connectionString
                    return! ctx.Ok ""
                | None -> 
                    return! ctx.NotFound {| Error =  "Not found" |}
            }
       )) |> ignore
