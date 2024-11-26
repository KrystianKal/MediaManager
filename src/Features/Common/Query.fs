module MediaManager.Features.Common.Query

open System.Threading.Tasks
open MediaManager.RopResult
open MediaManager.Messages

type SortDirection =
    | Asc
    | Desc
    
type PagingParams =
    {
        Offset: int
        Limit: int
    }

let trySingleR  = function
    | [] -> fail ResourceNotFound
    | [Success (x,msgs)] -> Success (x,msgs)
    | [Failure msgs] -> Failure msgs
    | _::_ -> fail ExpectedSingleButFoundMany
    
let trySingleRAsync (items: Task<RopResult<'T> list>) =
    task{
        let! items' = items
        return trySingleR items'
    }
    
// let trySingle (items: 'T list) : 'T option =
//     match items with
//     | [] -> None
//     | [x] -> Some x
//     | _::_ -> None
// let trySingleAsync (items: Task<'T list>) =
//     task {
//         let! items' = items
//         return trySingle items'
//     }
    


