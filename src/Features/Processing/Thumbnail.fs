module MediaManager.Features.Processing.Thumbnail

open System
open MediaManager
open MediaManager.Logger
open MediaManager.Models
open MediaManager.Models.Common
//TODO Global Quality value
//TODO lower resolution
//TODO Skip color analysis for mkv
    
let private formatTimestamp (seconds:float ) =
    let ts = System.TimeSpan.FromSeconds(seconds)
    // eg. "00:01:23.500"
    $"%02d{ts.Hours}:%02d{ts.Minutes}:%02d{ts.Seconds}.%03d{ts.Microseconds}"
    
let private generateUniqueThumbnailName (m:Media) =
    let timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
    $"{Path.value m.Path}\\Thumbnails\\{m.Name}_{Media.getExt m.Type}_{timestamp}_t"
let private generateImageThumbnail (inputFile:string) (outputFileName:string) =
    let outputFile = $"{outputFileName}.jpg"
    let success = Ffmpeg.StartProcess $"-i \"{inputFile}\" -q:v 14 \"{outputFile}\""
    if not success then
        log Error $"Failed generating image thumbnail for: {inputFile}"
    outputFile

//Some video formats don't work with -ss, therefore use fallback
let private generateVideoThumbnailWithFallback (inputFile:string) (timestamp: string) (outputFile:string) =
    let success = Ffmpeg.StartProcess $"-i \"{inputFile}\" -ss {timestamp} -frames:v 1 -q:v 14  \"{outputFile}\""
    let mutable firstTry = true
    if not success then
        log Information $"Failed generating video thumbnail for: {inputFile}; Trying fallback"
        firstTry <- false
        let fallbackSuccess = Ffmpeg.StartProcess $"-i \"{inputFile}\" -q:v 14 \"{outputFile}\""
        if not fallbackSuccess then
            log Error $"Failed generating fallback thumbnail for: {inputFile}"
    //TODO return option/result, as of now if a thumbnail will fail to generate it will still be inserted into db
    firstTry
        
        
let private generateVideoThumbnail (inputFile:string) (videoDuration: int) (outputFileName:string) =
    let outputFile = $"{outputFileName}.jpg"
    let middleTimestamp = formatTimestamp ((float videoDuration)/2.0 )
    generateVideoThumbnailWithFallback inputFile middleTimestamp outputFile |> ignore
    outputFile

let private generateMultipleVideoThumbnails (inputFile:string) (duration: int) (outputPrefix:string) count =
    let calculateTimestamp (startTime: float ) (interval : float) (index: int) =
        startTime + (float index * interval)
    let calculateInterval (duration:float) (count: int) =
        // skip last 2 seconds to avoid time overflow
        ( float duration - 2.0) / float (count - 1)
        
    let interval = calculateInterval duration count
    let mutable hadToFallback = false
    [|
       for i in 0 .. (count - 1) do
           if not hadToFallback then
               let formattedTimestamp =
                   (calculateTimestamp 1.0 interval i)
                   |> formatTimestamp
               let outputFile = $"{outputPrefix}_{i+1}.jpg"
               hadToFallback <- not(generateVideoThumbnailWithFallback inputFile formattedTimestamp outputFile)
               outputFile
    |]
        

let generateThumbnail (m: Media): Media =
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
    log Information $"Generated '{thumbnails.Length}' thumbnails for {Media.fullName m}"
    {m with Thumbnails = Set.ofArray thumbnails}

//TODO If thumbnail was deleted regenerate 
//TODO scan for thumbnails that are not assigned to any media and show error
let generateThumbnails (files: Media array): Media array =
    files
    |> Array.map generateThumbnail 
    