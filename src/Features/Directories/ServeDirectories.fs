module MediaManager.Features.Directories.ServeDirectories

open System
open System.IO
open MediaManager.Database.Dto.DirectoryDto
open Microsoft.AspNetCore.Builder
open MediaManager.RopResult
open MediaManager.Features.Common.Utils
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.FileProviders

let serveDirectories (app: WebApplication) (connectionString: string)  =
    let configureStaticFiles (directories: DirectoryDto list) =
        directories
        |> List.iter  ( fun dir ->
            let options = StaticFileOptions()
            options.FileProvider <- new PhysicalFileProvider(dir.Path)
            options.RequestPath <- createRequestPath(dir.Path) |> PathString
            app.UseStaticFiles(options) |> ignore
        )
        
    task{
        let! result =
            GetAllDirectories.query connectionString
            |> aggregateAsync
        match result with
        | Success (directories,_) -> configureStaticFiles directories
        | Failure _ -> failwith "Failed to serve directories"
    }
    
