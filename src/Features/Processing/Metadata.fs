module MediaManager.Features.Processing.Metadata
open System
open System.IO
open System.Threading.Tasks
open MediaManager
open MediaManager.Models
open MediaManager.Models.MediaFormats
open MediaManager.Models.Common.Path
open TagLib

type private ImageMetadata = {
    Width: int
    Height: int
}
type private VideoMetadata = {
    Width: int
    Height: int
    Duration: int 
    Bitrate: int 
}
type private Metadata =  |ImageMetadata of ImageMetadata | VideoMetadata of VideoMetadata

let private getVideoMetadata (file: File) =
    let properties = file.Properties
    {
        Width = properties.VideoWidth
        Height =  properties.VideoHeight
        Duration = (int properties.Duration.TotalSeconds)
        //TODO Use ffmpeg 
        Bitrate = 0
    }  
let private getImageMetadata (file: File) =
    let properties = file.Properties
    {
        Width = properties.PhotoWidth
        Height = properties.PhotoHeight
    }
    
let private addMetadata (fi: FileInfo) : FileInfo * Metadata =
    let path = fi.FullName
    use file = TagLib.File.Create path
    let mediaTypes = file.Properties.MediaTypes
    if  (mediaTypes &&& MediaTypes.Video) = MediaTypes.Video then
        fi, Metadata.VideoMetadata (getVideoMetadata file)
    elif (mediaTypes &&& MediaTypes.Photo) = MediaTypes.Photo then
        fi, Metadata.ImageMetadata (getImageMetadata file)
    else 
        failwith "Media type not supported; Filter Files beforehand"
    
let private waitForFileAvailability  (fi:FileInfo) (timeoutMs:int)= task{
    let stopwatch = System.Diagnostics.Stopwatch.StartNew()
    let mutable isAvailable = false
    while not isAvailable && stopwatch.ElapsedMilliseconds < timeoutMs do
        try
            // try to open file in read only mode to check for availability
            use _ = fi.Open(FileMode.Open,FileAccess.Read, FileShare.None)
            isAvailable <- true
        with
        | :? IOException ->
            Logger.log Logger.Warning $"File {fi.FullName} not available. Timeout: {stopwatch.ElapsedMilliseconds} / {timeoutMs} "
            do! Task.Delay(1000)
    return isAvailable
}
let private addMetadataWhenAvailable (fi:FileInfo)  = task{
    let timeoutMs = 100000 //100s timeout
    let! isAvailable = waitForFileAvailability fi timeoutMs
    if isAvailable then
        return addMetadata fi
    else
        Logger.log Logger.Warning $"Skipped {fi.FullName} not available after {timeoutMs}"
        return failwith $"File {fi.FullName} is not available."
}
let private createMedia ((fi, m): FileInfo*Metadata): Media = 
    let path = Path fi.DirectoryName
    let name = Path.GetFileNameWithoutExtension fi.Name
    let ext = fi.Extension.TrimStart('.').ToLowerInvariant()
    let createdAt = DateTimeOffset(fi.CreationTimeUtc)
    let size = fi.Length
    match m with
    | ImageMetadata im ->
        //Cannot be none since it was filtered beforehand
        let format = tryParseImageFormat ext |> Option.get
        Media.createImage path name format createdAt Set.empty im.Width im.Height size
    | VideoMetadata vm ->
        //Cannot be none since it was filtered beforehand
        let format = tryParseVideoFormat ext |> Option.get
        Media.createVideo path name format createdAt Set.empty vm.Width vm.Height size vm.Duration vm.Bitrate
        
let private filterFile (fi: FileInfo) =
    let ext = fi.Extension.TrimStart('.').ToLowerInvariant()
    let isImage = Option.isSome (tryParseImageFormat ext)
    let isVideo = Option.isSome (tryParseVideoFormat ext)
    isVideo || isImage
let private addMetadataAndCreateMedia (fi:FileInfo) = task{
    let! metadata = addMetadataWhenAvailable fi
    return createMedia metadata
    }
let scanFilesInDirectory (Path dir) =
    Directory.GetFiles(dir)
    |> Array.map FileInfo
    |> Array.filter filterFile
    |> Array.map addMetadataAndCreateMedia
    |> Task.WhenAll
let scanSingleFile (path:string) =
    FileInfo path
    |>  addMetadataAndCreateMedia