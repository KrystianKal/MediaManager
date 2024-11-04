module MediaManager.Features.Common.Query

open System.Threading.Tasks

type SortDirection =
    | Asc
    | Desc
    
type PagingParams =
    {
        Offset: int
        Limit: int
    }

let trySingle (items: 'T list) : 'T option =
    match items with
    | [] -> None
    | [x] -> Some x
    | _::_ -> None
let trySingleAsync (items: Task<'T list>) : Task<'T option> =
    task {
        let! list = items
        return list |> trySingle
    }
    


