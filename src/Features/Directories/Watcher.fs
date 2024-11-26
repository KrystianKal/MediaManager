module MediaManager.Features.Directories.Watcher

open System
open System.IO
open System.Threading.Channels
open MediaManager.Features.Directories.FileEvents
open MediaManager.Database.Dto.DirectoryDto
open MediaManager.RopResult

let private createWatcher (path:string) =
   let watcher = new FileSystemWatcher(path)
   watcher.IncludeSubdirectories <- false
   watcher.NotifyFilter <- NotifyFilters.FileName ||| NotifyFilters.DirectoryName
   watcher.EnableRaisingEvents <- true
   watcher
   
let private setupWatcherEvents (directoryId: Guid) (connectionString: string) (channel: Channel<FileEvent>)(watcher: FileSystemWatcher) =
    watcher.Created.Add( fun args ->
        FileCreatedEvent (args.FullPath,directoryId,connectionString)
        |> channel.Writer.TryWrite
        |> ignore
    )
    watcher.Deleted.Add( fun args ->
        FileDeletedEvent (args.FullPath,directoryId,connectionString)
        |> channel.Writer.TryWrite
        |> ignore
    )

let watchDirectory  (connectionString:string) (channel: Channel<FileEvent>) (dir:DirectoryDto)=
    if Path.Exists dir.Path then
        createWatcher dir.Path
        |> setupWatcherEvents dir.Id connectionString channel
        
let watchExistingDirectories (connectionString:string) (channel: Channel<FileEvent>)=
    let createWatchers directories =
        List.iter (watchDirectory connectionString channel) <| directories
        
    GetAllDirectories.query connectionString
    |> aggregateAsync
    <|!> createWatchers
    |> ignore

