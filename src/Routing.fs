module MediaManager.Routing

open System
open System.Threading.Tasks
open MediaManager.Features.Directories
open MediaManager.Features.Media
open MediaManager.Features.Media.Tags
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Mvc

type WebApplication with
    member app.RegisterEndpoints connectionString fileTaskQueue =
        GetAllMedia.RegisterGetEndpoint "/media" app connectionString 
        GetMediaById.RegisterGetEndpoint "/media/{id}" app connectionString
        Favorite.RegisterPostEndpoint "media/{id}/favorite" app connectionString
        Favorite.RegisterDeleteEndpoint "media/{id}/favorite" app connectionString
        AddTagsToMedia.RegisterPostEndpoint "/media/{id}/Tags" app connectionString
        RemoveTagFromMedia.RegisterDeleteEndpoint "/media/{id}/Tags/{tag}" app connectionString
        
        GetAllTags.RegisterGetEndpoint "/tags" app connectionString
        
        RegisterDirectory.RegisterPostEndpoint "/directories/{path}" app connectionString fileTaskQueue
        GetDirectoryById.RegisterGetEndpoint "/directories/{id}" app connectionString
        GetAllDirectories.RegisterGetEndpoint "/directories" app connectionString
