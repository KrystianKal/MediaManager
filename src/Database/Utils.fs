module MediaManager.Database.Utils.DatabaseUtils

open System.Reflection
open DbUp
open Npgsql.FSharp

let ApplyMigrations connectionString =
    EnsureDatabase.For.PostgresqlDatabase(connectionString);
    let upgrader =
        DeployChanges.To
            .PostgresqlDatabase(connectionString)
        // .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), script => script.StartsWith("SampleApplication.PostDeployment."), new SqlScriptOptions { ScriptType = ScriptType.RunAlways, RunGroupOrder = 3})
            .WithScriptsEmbeddedInAssembly(Assembly.GetCallingAssembly())
            .LogToConsole()
            .Build()
    upgrader.PerformUpgrade()
    
let private extractDbName (connectionString: string) =
   connectionString.Split(';')
       |> Array.tryFind (fun part -> part.StartsWith("Database=", System.StringComparison.OrdinalIgnoreCase))
       |> Option.map (fun part -> part.Substring(part.IndexOf("=") + 1))
       |> Option.defaultValue ""
       
let private DropDatabase connectionString databaseName=
    let terminateConnectionQuery = @$"
        SELECT pg_terminate_backend(pg_stat_activity.pid)
        FROM pg_stat_activity
        WHERE pg_stat_activity.datname = '{databaseName}'
        AND pid <> pg_backend_pid();"
    let dropDatabaseQuery = $"DROP DATABASE IF EXISTS {databaseName}"
    connectionString
        |> Sql.connect
        |> Sql.query dropDatabaseQuery
        |> Sql.executeNonQuery
        // |> Sql.parameters ["db_name", Sql.text databaseName]
        // |> Sql.executeNonQuery
        // |> Sql.executeTransaction [
        //     terminateConnectionQuery, []
        //     dropDatabaseQuery, []
        // ]
    |> ignore
let private CreateDatabase connectionString databaseName =
    let createDatabaseQuery = $"CREATE DATABASE {databaseName}"
    connectionString
        |> Sql.connect
        |> Sql.query createDatabaseQuery
        |> Sql.executeNonQuery
    
let PurgeDatabase connectionString = 
    let databaseName = extractDbName connectionString
    let postgresConnectionString = connectionString.Replace(databaseName, "postgres")
    DropDatabase postgresConnectionString databaseName
    CreateDatabase postgresConnectionString databaseName

