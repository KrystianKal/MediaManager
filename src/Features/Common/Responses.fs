module MediaManager.Features.Common.Responses

open System
open System.Threading.Tasks
open MediaManager.Messages
open MediaManager.RopResult
open Microsoft.AspNetCore.Mvc

type EndpointDelegate = Func<Task<IActionResult>>
type EndpointDelegate<'T1> = Func<'T1,Task<IActionResult>>
type EndpointDelegate<'T1,'T2> = Func<'T1,'T2,Task<IActionResult>>
let ok content = OkObjectResult(content) :> IActionResult
let created (uri:string) content = CreatedResult(uri, content) :> IActionResult
             
let InternalServerErrorResult msgs =
        let res = ObjectResult(msgs)
        res.StatusCode <- 500
        res :> IActionResult
type ResponseMessage =
    | NotFound
    | BadRequest of string
    | InternalServerError of string
    
let classify (msg: Message)=
    match msg with 
    | InvalidGuidFormat
    | NoRowsAffected ->
        BadRequest $"{msg}"
    | InvalidPath p ->
        BadRequest p
    | ResourceNotFound ->
        NotFound
    | ExpectedSingleButFoundMany
    | DeserializationFailed ->
        InternalServerError $"{msg}"
        
//TODO test
let primaryMsg msgs =
    msgs
    |> List.map classify
    |> List.head // can assume at least one
let badRequestToString msgs =
    msgs
    |> List.map classify
    |> List.choose (function BadRequest s -> Some s | _ -> None)
    |> List.map (sprintf "ValidationError: %s;")
    |> List.reduce (+)
    

let toHttpResult (result:Task<RopResult<IActionResult>>) : Task<IActionResult> =
    task{
    match! result with 
    | Success (x,_) -> return x
    | Failure msgs ->
        return
            match primaryMsg msgs with
            | NotFound ->
                NotFoundResult()
                :> IActionResult
            | BadRequest _->
                BadRequestObjectResult(badRequestToString msgs)
                :> IActionResult
            | InternalServerError ise ->
                InternalServerErrorResult ise
    }
