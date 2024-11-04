module MediaManager.Features.Common.Responses

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Http

type EndpointDelegate = Func<HttpContext,Task>
type EndpointDelegate<'T> = Func<HttpContext,'T,Task>
type EndpointDelegate<'T1, 'T2> = Func<HttpContext,'T1,'T2,Task>

type HttpContext with
    member ctx.WithStatusCode (sc: int) (content: obj) =
        ctx.StatusCode sc
        ctx.WriteJson content
    member ctx.Ok (content:obj) = ctx.WithStatusCode StatusCodes.Status200OK content
    member ctx.Created (content:obj) = ctx.WithStatusCode StatusCodes.Status201Created content
    member ctx.NotFound (content:obj) = ctx.WithStatusCode StatusCodes.Status404NotFound content
    member ctx.BadRequest (content:obj) = ctx.WithStatusCode StatusCodes.Status400BadRequest content
    
    member ctx.WriteJson (o:obj) =
      ctx.Response.WriteAsJsonAsync(o)
    member ctx.StatusCode sc =
       ctx.Response.StatusCode <- sc
