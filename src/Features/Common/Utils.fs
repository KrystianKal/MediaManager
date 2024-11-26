module MediaManager.Features.Common.Utils

open System
open System.IO

let createRequestPath (path:string) =
   let trimmed = Path.TrimEndingDirectorySeparator(path).AsSpan()
   let colonIndex = trimmed.IndexOf(':')
   if colonIndex = -1 then
       failwith "Unexpected path format when configuring static directories; No ':' found"
   let withoutDrive = trimmed.Slice(colonIndex + 1)
   "/StaticFiles" + withoutDrive.ToString().Replace("\\","/")
