module MediaManager.Features.Directories.DirectoryMappers

open MediaManager.Database.Dto
open MediaManager.Database.Dto.DirectoryDto
open MediaManager.Models.Common.Path
open MediaManager.Models.Directory

let fromDto (dto: DirectoryDto): Directory =
    {
        Id = dto.Id
        Size = dto.Size
        Path = Path dto.Path
        MediaFileCount = dto.FileCount
    }

let toDto directory : DirectoryDto=
    {
        Id = directory.Id
        Size = directory.Size
        Path = value directory.Path
        FileCount = directory.MediaFileCount
    }
