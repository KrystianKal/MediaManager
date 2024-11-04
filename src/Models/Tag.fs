namespace MediaManager.Models

open System
open System.Text.Json.Serialization

type Tag = {
    Id: Guid
    Name: string
    [<JsonPropertyName "usage_count">]
    UsageCount: int
}

// module Tag =
//     let create id name =
//         {
//             Id = id 
//             Name = name
//             UsageCount = 0 
//         }