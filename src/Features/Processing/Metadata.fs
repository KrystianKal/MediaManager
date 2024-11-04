module MediaManager.Features.Processing.Metadata
open System
open System.IO
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
    try
        use file = TagLib.File.Create path
        let mediaTypes = file.Properties.MediaTypes
        if  (mediaTypes &&& MediaTypes.Video) = MediaTypes.Video then
            fi, Metadata.VideoMetadata (getVideoMetadata file)
        elif (mediaTypes &&& MediaTypes.Photo) = MediaTypes.Photo then
            fi, Metadata.ImageMetadata (getImageMetadata file)
        else 
            failwith "Media type not supported; Filter Files beforehand"
    with
        //TODO retry on fail?  Why would it fail?
        | _ -> failwith $"Unable to fetch metadata of {path}"
  

let private createMedia ((fi, m): FileInfo*Metadata): Media = //has to be valid
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
let ScanFilesInDirectory (Path dir) =
    Directory.GetFiles(dir)
    |> Array.map FileInfo
    |> Array.filter filterFile
    |> Array.map (addMetadata >> createMedia)