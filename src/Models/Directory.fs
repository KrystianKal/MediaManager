module MediaManager.Models.Directory

open System
open MediaManager.Models.Common.Path


//TODO what will happen if directory name is changed?
//if a directory is not found, user will be asked
//whether the directory was deleted or was renamed
type Directory = {
    Id:Guid
    Path: Path
    MediaFileCount: int
    Size: int64
}

let create path =
    {
        Id= Guid.NewGuid()
        Path= path
        MediaFileCount = 0 
        Size = 0 
    }
