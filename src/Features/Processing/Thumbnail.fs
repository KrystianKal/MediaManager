module MediaManager.Features.Processing.Thumbnail

open System
open MediaManager.Models
open MediaManager.Models.Common
//TODO Global Quality value
//TODO lower resolution
//TODO Skip color analysis for mkv
//TODO Add logging
//TODO fail or something if thumbnail already do be existing
    
let private formatTimestamp (seconds:float ) =
    let ts = System.TimeSpan.FromSeconds(seconds)
    // eg. "00:01:23.500"
    $"%02d{ts.Hours}:%02d{ts.Minutes}:%02d{ts.Seconds}.%03d{ts.Microseconds}"
    
let private generateUniqueThumbnailName (m:Media) =
    let timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    $"{Path.value m.Path}\\Thumbnails\\{m.Name}_{Media.getExt m.Type}_{timestamp}_t"
let private generateImageThumbnail (inputFile:string) (outputFileName:string) =
    let outputFile = $"{outputFileName}.jpg"
    Ffmpeg.StartProcess $"-i {inputFile} -q:v 14 {outputFile}"
    outputFile
    
let private generateVideoThumbnail (inputFile:string) (videoDuration: int) (outputFileName:string) =
    let outputFile = $"{outputFileName}.jpg"
    let middleTimestamp = formatTimestamp ((float videoDuration)/2.0 )
    Ffmpeg.StartProcess $"-ss {middleTimestamp} -i {inputFile} -frames:v 1 -q:v 14 {outputFile}" 
    outputFile

let private generateMultipleVideoThumbnails (inputFile:string) (duration: int) (outputPrefix:string) n =
    // First frame is taken on the first second
    let interval = ( float duration - 1.0) / float (n - 1)
    [|
       for i in 0 .. (n - 1) do
           let formattedTimestamp = formatTimestamp (1.0 + (float i * interval))
           let outputFile = $"{outputPrefix}_{i+1}.jpg"
           Ffmpeg.StartProcess $"-ss {formattedTimestamp} -i {inputFile} -frames:v 1 -q:v 14 {outputFile}"
           outputFile
    |]
        

//TODO If thumbnail was deleted regenerate 
//single thumbnail directory -> same file name conflict
//TODO detect media deletion
//TODO delete thumbnail after source media was deleted
//TODO scan for thumbnails that are not assigned to any media and show erorr
let generateThumbnails (files: Media array) =
    files
    |> Array.map ( fun m ->
        let generateThumbnailsForVideo (v: Video) =
            if v.DurationSeconds <= 3 then
                [| generateVideoThumbnail (Media.fullName m) v.DurationSeconds (generateUniqueThumbnailName m) |]
            else
                //TODO Calculate N based on video length
                generateMultipleVideoThumbnails  (Media.fullName m) v.DurationSeconds (generateUniqueThumbnailName m) 5
            
        let thumbnails =
            match m.Type with 
            | Image _ -> [| generateImageThumbnail (Media.fullName m) (generateUniqueThumbnailName m) |]
            | Video v -> generateThumbnailsForVideo v
        {m with Thumbnails = Set.ofArray thumbnails}
    )
    