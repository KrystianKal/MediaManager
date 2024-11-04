module MediaManager.Features.Media.DAL.MediaMappers

open System
open MediaManager.Database.Dto.MediaDto
open MediaManager.Models
open MediaManager.Models.Common
open MediaManager.Models.MediaFormats

let mediaToDto (media: Media) : MediaDto =
    let (mediaType, format, duration, bitRate) =
        match media.Type with
        | Image img -> "image", imageFormatAsString img.Format, None, None
        | Video vid -> "video", videoFormatAsString vid.Format, Some vid.DurationSeconds, Some vid.BitRate

    {
        Id = media.Id
        Path = Path.value media.Path
        Name = media.Name
        CreatedAt = media.CreatedAt.DateTime
        ViewCount = media.ViewCount
        Tags = media.Tags
        Thumbnails = media.Thumbnails
        IsFavorite = media.IsFavorite
        Width = media.Width
        Height = media.Height
        FileSize = media.FileSize
        Type = mediaType
        Format = format
        DurationSeconds = duration
        BitRate = bitRate
    }

let fromDto (dto: MediaDto) : Media =
    match dto.Type.ToLowerInvariant() with
    | "image" ->
        match tryParseImageFormat dto.Format with
        | Some format ->
            Media.createImage  
                (Path.Path dto.Path)
                dto.Name
                format
                (DateTimeOffset(dto.CreatedAt))
                dto.Tags
                dto.Width
                dto.Height
                dto.FileSize
        | None -> failwith $"Invalid image format: {dto.Format}"
    | "video" ->
        match tryParseVideoFormat dto.Format with
        | Some format ->
            match dto.DurationSeconds, dto.BitRate with 
                | Some duration, Some bitrate ->
                Media.createVideo  
                    (Path.Path dto.Path)
                    dto.Name
                    format
                    (DateTimeOffset(dto.CreatedAt))
                    dto.Tags
                    dto.Width
                    dto.Height
                    dto.FileSize
                    duration
                    bitrate
                | _ -> failwith "Missing duration or bitrate"    
        | None -> failwith $"Invalid video format: {dto.Format}"
    | invalidMediaType -> failwith $"Invalid media type: {invalidMediaType}"
