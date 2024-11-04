module MediaManager.Database.Dto.DirectoryDto

open System

type DirectoryDto = {
    Id: Guid
    Path: string
    FileCount: int
    Size: int64
}

