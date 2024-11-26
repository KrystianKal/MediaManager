module MediaManager.Models.Common.Path

open System
open System.IO
open MediaManager.RopResult
open MediaManager.Messages

type Path = Path of string
let value (Path path) = path
let private isLetter c = Char.IsLetter c
let private containsInvalidCharacters (p:string) =
    let invalidFileChars = Path.GetInvalidFileNameChars()
    let invalidPathChars = Path.GetInvalidPathChars()
    let allInvalidChars = Array.append invalidFileChars invalidPathChars |> Array.distinct
    p |> String.exists (fun c -> Array.contains c allInvalidChars)
let private endsWithSlash (p:string) = p.EndsWith(@"/") || p.EndsWith(@"\")
let private hasConsecutiveSlashes (p:string)=
    p.Contains(@"\\") || p.Contains(@"//")
let private isValidPathSection (s:string) =
    not (String.IsNullOrWhiteSpace s)
    && not (containsInvalidCharacters s)
    && not (s.EndsWith ".")
    && not (s.EndsWith " ")
    && not (s.EndsWith " ")
let private isValidDriveSection (s:string) =
    if s.Length <> 2 then false
    elif not (isLetter s[0]) then false
    elif s[1] <> ':' then false
    else true
let private validatePath (p:string array) = Array.forall isValidPathSection p

let private segmentPath (p: string) =
    p.Split([|'/';'\\'|], System.StringSplitOptions.RemoveEmptyEntries)
let exists (path:Path) =
    Directory.Exists(value path)
let createAbsolute path =
    if String.IsNullOrWhiteSpace path then
        fail (InvalidPath "Path cannot be empty")
    else 
    let parts = segmentPath path
    if path.Length < 4 then
        fail (InvalidPath  "Path is too short. Example shortest path: 'C:\\'")
    elif hasConsecutiveSlashes path then 
        fail (InvalidPath  @"Path has consecutive slashes \\'")
    elif not (isLetter path[0] )then
        fail (InvalidPath  "Path must start with a letter")
    elif not (endsWithSlash path ) then
        fail (InvalidPath  "Path must end with slash")
    elif not(isValidDriveSection parts[0]) then
        fail (InvalidPath  "Invalid drive section")
    //skip drive section
    elif not (validatePath parts[1..]) then 
        fail (InvalidPath  "Path is not valid")
    else
        succeed (Path path)
let createRelative path = 
    if String.IsNullOrWhiteSpace path then
        fail (InvalidPath  "Path cannot be empty")
    else 
    let parts = segmentPath path
    
    if path.Length < 2 then
        fail (InvalidPath  "Path is too short. Example shortest path: '\\p")
    elif hasConsecutiveSlashes path then 
        fail (InvalidPath  @"Path has consecutive slashes \\'")
    elif not (validatePath parts) then 
        fail (InvalidPath  "Path is not valid")
    elif not (endsWithSlash path ) then
        fail (InvalidPath  "Path must end with slash")
    else
        succeed (Path path)