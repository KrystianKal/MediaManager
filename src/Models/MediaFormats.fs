module MediaManager.Models.MediaFormats

type ImageFormat = 
  | Png
  | Jpg
  | Jpeg
  | Jfif
  | Gif
  | Webp
type VideoFormat =
  | Mp4
  | M4v
  | Webm
  | Mkv
let imageFormatAsString = function
    | Png -> "png"
    | Jpg -> "jpg"
    | Jpeg -> "jpeg"
    | Jfif -> "jfif"
    | Gif -> "gif"
    | Webp -> "webp"

let videoFormatAsString = function
    | Mp4 -> "mp4"
    | M4v -> "m4v"
    | Webm -> "webm"
    | Mkv -> "mkv"

let tryParseImageFormat = function
    | "png" -> Some Png
    | "jpg" -> Some Jpg
    | "jpeg" -> Some Jpeg
    | "jfif" -> Some Jfif
    | "gif" -> Some Gif
    | "webp" -> Some Webp
    | _ -> None
    
let tryParseVideoFormat = function
    | "mp4" -> Some Mp4
    | "m4v" -> Some M4v
    | "webm" -> Some Webm
    | "mkv" -> Some Mkv
    | _ -> None
