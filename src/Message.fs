module MediaManager.Messages

open System

type Message =
    // Validation errors
    | InvalidGuidFormat
    | InvalidPath of string
    // Exposed errors
    | ResourceNotFound
    | ExpectedSingleButFoundMany
    // | DatabaseUnableToProcess
    | NoRowsAffected
    // Internal errors
    | DeserializationFailed
    // Domain events
    // | MediaViewed of Guid

