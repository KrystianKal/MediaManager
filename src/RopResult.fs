module MediaManager.RopResult

open System.Threading.Tasks
open Messages

//Railway oriented programming
type RopResult<'TSuccess> =
    | Success of 'TSuccess * Message list
    | Failure of Message list

let succeed x = Success (x, [])
let succeedWithMsg x msg= Success (x, [msg])
let fail msg = Failure [msg]

let either fSuccess fFailure = function
    | Success (x,msg) -> fSuccess (x,msg)
    | Failure errors -> fFailure errors
let mergeMessages msgs result =
    let fSuccess (x,msgs2) =
        Success( x,msgs@msgs2 )
    let fFailure msgs2 =
        Failure( msgs@msgs2)
    either fSuccess fFailure result

///given f: a -> RopResult<>
///apply it if the result is Success
///merge existing messages with new result
let bind f result =
    let fSuccess (x,msgs) =
         f x |> mergeMessages msgs
    let fFailure errors =
        Failure errors
    either fSuccess fFailure result
///given f: a -> RopResult<> and Task<RopResult<>>
///apply it if the result is Success
///merge existing messages with new result
let bindAsync f resultTask = task{
    let! result = resultTask
    return bind f result
}
    
///given 'f: a -> Task<RopResult<>>'
///apply it if the result is Success and 
///merge existing messages with new result

let bindWithAsync (f: 'a -> Task<RopResult<'b>>) result = task{
    let fSuccess (x,msgs) = task{
         let! res = f x
         return res |> mergeMessages msgs
     }
    let fFailure errors = task {
        return Failure errors
    }
    return! either fSuccess fFailure result
    }
    
///given 'f: a -> Task<RopResult<>>' and Task<RopResult> 
///apply it if the result is Success and 
///merge existing messages with new result
let bindAsyncWithAsync (f: 'a -> Task<RopResult<'b>>) (resultTask:Task<RopResult<'a>>) = task{
    let! result = resultTask
    return! bindWithAsync f result
    }

let (>>=) result f = bind f result
let (>>=|) result f = bindWithAsync f result
let (|>>=) result f = bindAsync f result
// let (|>>=) result f = bindWithAsync f result
// let (>>=|) result f = bindAsync f result
let (|>>=|) result f = bindAsyncWithAsync f result
/// given a function wrapped in a result
/// and a value wrapped in a result
/// apply the function to the value only if both are Success
let apply f result =
    match f,result with
    | Success (f,msgs1), Success (x,msgs2) -> 
        (f x, msgs1@msgs2) |> Success 
    | Failure errs, Success (_,msgs) 
    | Success (_,msgs), Failure errs -> 
        errs @ msgs |> Failure
    | Failure errs1, Failure errs2 -> 
        errs1 @ errs2 |> Failure
let applyAsync f resultTask =
     task{
         let! result = resultTask
         return apply f result
     }
// infix operator
let (<*>) = apply
let (<|*>) = applyAsync

// given a function that transforms a value
// wrap it in RopResult and apply if Success
let lift f result = (succeed f) <*> result

// given RopResult task
// await it and lift 
let liftAsync f resultTask =
     task{
         let! result = resultTask
         return lift f result
     }
let lift2 f r1 r2 = (lift f r1) <*> r2

/// |> apply
// given a function that transforms a value
// wrap it in RopResult and apply if Success
let (<!>) result f = lift f result
/// |> applyAsync
// given RopResult task
// await it and lift 
let (<|!>) result f  = liftAsync f result 
// synonyms
let map = lift
let mapAsync = liftAsync

///given a RopResult, call a sync function on the success branch
///and pass through the result
let tapOnSuccess (f: 'a -> unit) result =
    let fSuccess (x,msgs) =
        do f x
        Success (x,msgs)
    let fFailure errors = Failure errors
    either fSuccess fFailure result
let (>>-) result f = tapOnSuccess f result

///given a RopResult task, call a sync function on the success branch
///and pass through the result
let tapOnSuccessAsync f resultTask = task{
    let! result =  resultTask
    return tapOnSuccess f result
    }
let (|>>-) result f = tapOnSuccessAsync f result
    
///given a RopResult, call a task function on the success branch
///and pass through the result
let tapOnSuccessWithAsync (fTask: 'a -> Task<'b>) result = task{
    let fSuccess (x,msgs) = task{
        let! _ = fTask x
        return Success (x,msgs)
    }
    let fFailure errors = task {return Failure errors}
    return! either fSuccess fFailure result
    }
let (>>-|) result f = tapOnSuccessWithAsync f result

///given a RopResult task, call a task function on the success branch
///and pass through the result
let tapOnSuccessAsyncWithAsync (fTask: 'a -> Task<'b>) resultTask = task{
    let! result = resultTask
    return! tapOnSuccessWithAsync fTask result
    }
let (|>>-|) result f = tapOnSuccessAsyncWithAsync f result

///given a RopResult, call a unit function on the failure branch
///and pass through the result
let tapOnFailure f result =
    let fSuccess(x,msgs) = Success (x,msgs)
    let fFailure errors =
        f errors
        Failure errors
    either fSuccess fFailure result
let (>-) result f = tapOnFailure f result

///given a RopResult task, call a unit function on the failure branch
///and pass through the result
let tapOnFailureAsync f resultTask =task{
    let! result = resultTask
    return tapOnFailure f result
    }
let (|>-) result f = tapOnFailureAsync f result

let mapMessages f result =
    match result with
    | Success (x,msgs) ->
        let msgs' = List.map f msgs
        Success (x, msgs')
    | Failure (msgs) ->
        let msgs' = List.map f msgs
        Failure msgs'

/// aggregate RopResult<'a> list into RopResult<'a list>
/// if any of the RopResults is a failure
/// will return first failure
let aggregate (results: RopResult<'T> list): RopResult<'T list> =
    match results |> List.tryFind( function Failure _ -> true | _ -> false) with
    | Some (Failure msg) -> Failure msg
    | _ ->
        results
        |> List.choose( function Success(value,_) -> Some value | _ -> None)
        |> succeed
        
        
let aggregateAsync (resultsTask: Task<RopResult<'T> list>): Task<RopResult<'T list>> = task{
    let! results = resultsTask
    return aggregate results
    }


