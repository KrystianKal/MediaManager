module MediaManager.Features.Common.Parsers

open System

let tryParseGuid (s: string) =
    match Guid.TryParse s with
    | true, guid -> Some guid
    | false, _ -> None

