module MediaManager.Models.Directory

open System
open MediaManager.Models.Common.Path


//TODO what will happen if directory name is changed?
//at some point show error, allow for changing names
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
