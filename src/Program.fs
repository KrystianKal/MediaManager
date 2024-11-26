namespace MediaManager

#nowarn "20" //remove the need for |> ignore

open System.Threading.Channels
open MediaManager.Database.Utils.DatabaseUtils
open MediaManager.Features.Directories
open MediaManager.Features.Directories.FileEvents
open MediaManager.Features.Directories.FileHostedService
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Routing

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)
        
        let connectionString = builder.Configuration.GetConnectionString("db")
        
        // PurgeDatabase <| connectionString  
        
        let migrationResult = ApplyMigrations connectionString
        let fileTaskQueue = Channel.CreateUnbounded<FileEvent>()
        Watcher.watchExistingDirectories connectionString fileTaskQueue
        
        builder.Services.AddHostedService<FileHostedService>(
            fun _ ->
                new FileHostedService(fileTaskQueue)
            )
        builder.Services.AddControllers()
        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen()

        let app = builder.Build()
        
        let task = ServeDirectories.serveDirectories app connectionString
        task.Wait()
        

        if app.Environment.IsDevelopment() then
            app.UseSwagger() |> ignore
            app.UseSwaggerUI() |> ignore

        // app.UseHttpsRedirection()
       
        app.RegisterEndpoints connectionString fileTaskQueue
        
        app.MapControllers()
        
        app.Run() 

        exitCode