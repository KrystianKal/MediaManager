module MediaManager.Features.Directories.FileHostedService

open System.Threading.Tasks
open MediaManager.Features.Media
open MediaManager.Features.Processing.Metadata
open System.Threading.Channels
open MediaManager.Models
open MediaManager.RopResult
open MediaManager.Logger
open MediaManager.Features.Processing.Thumbnail
open MediaManager.Features.Directories.FileEvents
open Microsoft.Extensions.Hosting

//todo add cancellationToken
// generate thumbnails
//todo make it bound by the amount of threads 
// type private FileTaskQueue() =
//     let _queue = Channel.CreateUnbounded<FileEvent>()
//     member this.QueueItem (fileEvent:FileEvent) =
//         task{
//             do! _queue.Writer.WriteAsync fileEvent
//         }
//     member this.DequeueItem =
//         task{
//             return! _queue.Reader.ReadAsync()
//         }

let private fileCreatedHandler path directoryId connectionString =task{
    let! media =  scanSingleFile path
    return!
        [ media |> generateThumbnail ]
        |> (AddMedia.addMedia connectionString directoryId)
        |>>- logSuccess Information "File created event" (fun m -> Media.fullName m.Head)
        :> Task
    } 
let private fileDeletedHandler path directoryId connectionString =
    log Information $"File deleted event: {path}"
    (RemoveMedia.removeMedia connectionString directoryId path)
    
type FileHostedService(queue) =
    inherit BackgroundService()
    let _queue: Channel<FileEvent> = queue
    override this.ExecuteAsync(stoppingToken) = task{
        while not stoppingToken.IsCancellationRequested do
            match! _queue.Reader.ReadAsync() with
            | FileCreatedEvent (path, directoryId, connectionString) ->
                do! fileCreatedHandler path directoryId connectionString
            | FileDeletedEvent (path, directoryId, connectionString) ->
                do! fileDeletedHandler path directoryId connectionString
        }
