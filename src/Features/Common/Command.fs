module MediaManager.Features.Common.Command

open System.Threading.Tasks
open MediaManager.RopResult
open MediaManager.Messages
open Npgsql.FSharp

let private anyAffectedRows (rows: int) =
    if rows > 0 then
        succeed true
    else
        fail NoRowsAffected
let private anyAffectedRowsAsync (rows: Task<int>) =
    task{
        let! rows' = rows
        return anyAffectedRows rows'
    }

/// return RopResult<true> if any rows were affected
let execute (props: Sql.SqlProps) =
    Sql.executeNonQueryAsync props
    |> anyAffectedRowsAsync
    