module MediaManager.Features.Directories.FileEvents

open System

/// path, directoryId, connectionString
type FileEvent =
    | FileDeletedEvent of string*Guid*string
    | FileCreatedEvent of string*Guid*string
