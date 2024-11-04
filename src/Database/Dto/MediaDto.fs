module MediaManager.Database.Dto.MediaDto

open System
open MediaManager.Models

type MediaDto = {
    Id: Guid
    Path: string
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
