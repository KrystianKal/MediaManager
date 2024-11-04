module MediaManager.Routing

open MediaManager.Features.Directories
open MediaManager.Features.Media
open MediaManager.Features.Media.Tags
open Microsoft.AspNetCore.Builder

type WebApplication with
    member app.RegisterEndpoints connectionString =
        GetAllMedia.RegisterEndpoint "/media" app connectionString 
        GetMediaById.RegisterEndpoint "/media/{id}" app connectionString
        Favorite.RegisterEndpoint "media/{id}/favorite" app connectionString
        AddTagsToMedia.RegisterEndpoint "/media/{id}/Tags" app connectionString
        RemoveTagFromMedia.RegisterEndpoint "/media/{id}/Tags/{tag}" app connectionString
        GetAllTags.RegisterEndpoint "/tags" app connectionString
        RegisterDirectory.RegisterEndpoint "/directories/{path}" app connectionString
