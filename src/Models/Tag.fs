namespace MediaManager.Models

open System
open System.Text.Json.Serialization

type Tag = {
    Id: Guid
    Name: string
    [<JsonPropertyName "usage_count">]
    UsageCount: int
}
