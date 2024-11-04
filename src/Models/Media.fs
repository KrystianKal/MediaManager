namespace MediaManager.Models

open System
open MediaManager.Models.Common.Path
open MediaManager.Models.MediaFormats


type FilePath = private FilePath of string  
//TODO Validate isFile and file extension

type Image = {
  Format: ImageFormat
}

type Video = {
  Format: VideoFormat
  DurationSeconds: int 
  BitRate: int
}

type MediaType =
  | Image of Image
  | Video of Video

//TODO delete Name column from database
//combine path+name into path
type Media = {
  Id: Guid
  //path without file name
  Path: Path
  //file name without ext
  Name: string
  CreatedAt: DateTimeOffset
  ViewCount: int
  Tags: Tag Set
  Thumbnails: string Set
  IsFavorite: bool
  Width: int
  Height: int
  FileSize: int64
  Type: MediaType
  //TODO Implement color metadata (preferably from thumbnails)
  // maybe only for images, in case of vide do it from thumbnails (skip first and last few frames )
  // colorProfile
  // colorDistribution
}

module Media =
  let private initializeMedia path name createdAt tags width height fileSize mediaType = {
    Id = Guid.NewGuid()
    Path = path
    Name = name
    CreatedAt = createdAt
    ViewCount = 0
    Tags = tags
    IsFavorite = false
    Width = width
    Height = height
    FileSize = fileSize
    Type = mediaType
    Thumbnails = Set.empty 
  }
 
  let private createImageType format: Image = { Format = format }
  let private createVideoType format duration bitrate: Video = {
    Format = format
    DurationSeconds = duration
    BitRate = bitrate 
  }
  
  let createImage path name format  createdAt tags width height fileSize = 
    initializeMedia path name createdAt tags width height fileSize (MediaType.Image (createImageType format))
  let createVideo path name format createdAt tags width height fileSize duration bitrate = 
    initializeMedia path name createdAt tags width height fileSize (MediaType.Video (createVideoType format duration bitrate))
  let getExt =  function
      | Image i -> imageFormatAsString i.Format
      | Video v -> videoFormatAsString v.Format
  let fullName m =
    $"{value m.Path}\\{m.Name}.{getExt m.Type }"
  // let nameWithoutExt (n:string) =
  //   n.Split('.')[0]
      
  let incrementViewCount media = { media with ViewCount = media.ViewCount + 1 }
  
  let setFavorite isFavorite media = { media with IsFavorite = isFavorite }
  
  //TODO If needed change to Id comparison to avoid duplicate tags
  let addTags tags media = { media with Tags = Set.union media.Tags tags }

  let removeTags tags media = { media with Tags = Set.difference media.Tags tags }
