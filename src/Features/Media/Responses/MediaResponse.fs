module MediaManager.Features.Media.Responses.MediaResponse

open System
open MediaManager.Database.Dto.MediaDto
open MediaManager.Models
open MediaManager.Features.Common.Utils

type MediaResponse = {
    Id: Guid
    FullName: string
    Name: string
    CreatedAt: DateTime
    ViewCount: int
    Tags: Tag Set
    Thumbnails: string Set
    IsFavorite: bool
    Width: int
    Height: int
    FileSize: int64
    Type: string
    Format: string
    DurationSeconds: int option
    BitRate: int option
}

let fromDto (dto: MediaDto) =
    {
        Id = dto.Id
        FullName = createRequestPath dto.Path + "/" + dto.Name + "." + dto.Format
        Name = dto.Name
        CreatedAt = dto.CreatedAt
        ViewCount = dto.ViewCount
        IsFavorite = dto.IsFavorite
        Tags = dto.Tags  
        Thumbnails = dto.Thumbnails |> Set.map createRequestPath
        Width = dto.Width
        Height =  dto.Height
        FileSize = dto.FileSize
        Format = dto.Format
        Type = dto.Type
        DurationSeconds = dto.DurationSeconds
        BitRate = dto.BitRate
    }