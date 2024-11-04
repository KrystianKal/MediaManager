module MediaManager.Database.Serializers

open System.Text.Json
open MediaManager.Database.Dto.MediaDto
open MediaManager.Database.Dto.DirectoryDto
open MediaManager.Models


let deserializeDirectory (read:RowReader) = {
    Id = read.uuid "directory_id"
    Path = read.text "directory_path" 
    FileCount = read.int "directory_file_count" 
    Size = read.int64 "directory_size" 
}
let deserializeTag (read:RowReader) =
    {
        Id = read.uuid "id"
        Name = read.text "name"
        UsageCount = read.int "usage_count"
    }
let deserializeTags (read:RowReader) =
    read.text "tags"
        |> fun json ->
            let options = JsonSerializerOptions()
            options.PropertyNameCaseInsensitive <- true
            JsonSerializer.Deserialize<Tag array> (json, options)
        |> Set.ofArray
    
let deserializeMedia (read:RowReader) : MediaDto = {
    Id = read.uuid "id"
    Path = read.text "directory_path" 
    Name = read.text "name"
    CreatedAt = read.dateTime "created_at"
    ViewCount = read.int "view_count"
    IsFavorite = read.bool "is_favorite"
    Tags = deserializeTags read
    Thumbnails = read.stringArray "thumbnails" |> Set.ofArray
    Width = read.int "width" 
    Height =  read.int "height"
    FileSize = read.int64 "file_size"
    Format = read.text "format"
    Type = read.text "media_type"
    DurationSeconds = read.intOrNone "duration_seconds"
    BitRate = read.intOrNone "bit_rate" 
}
