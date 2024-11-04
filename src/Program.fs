namespace MediaManager

#nowarn "20" //remove the need for |> ignore

open MediaManager.Database.Utils.DatabaseUtils
open MediaManager.Features.Common.Responses
open MediaManager.Features.Processing
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
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

        builder.Services.AddControllers()
        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen()

        let app = builder.Build()

        if app.Environment.IsDevelopment() then
            app.UseSwagger() |> ignore
            app.UseSwaggerUI() |> ignore

        app.UseHttpsRedirection()
       
        //TODO Pass in logger?
        //TODO Validate path, create FilePath
        app.RegisterEndpoints connectionString
        // app.MapGet("/tt",
        //    EndpointDelegate<string>( fun(ctx: HttpContext) dirPath ->
        //        task{
        //        // let dir = @"C:\Users\Krystian\source\repos\MediaManager\src\Sandbox"
        //        // let x = Metadata.ScanFilesInDirectory dirPath
        //        // let z = Thumbnail.generateThumbnails x
        //        // printf "%d" z.Length
        //        // z.Length |> ctx.WriteJson 
        //        }
        // ))
        
        app.MapControllers()
        
        app.Run() 

        exitCode