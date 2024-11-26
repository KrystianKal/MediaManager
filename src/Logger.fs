module MediaManager.Logger
open Serilog

type LogLevel =
    | Error
    | Warning
    | Information
let private logger =
    Serilog.LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger()

let log level message =
    match level with
    | Error -> logger.Error(message)
    | Warning -> logger.Warning(message)
    | Information -> logger.Information(message)

let logFailure level message =
     fun errors ->
         log level $"%s{message}: %A{errors}"
let logSuccess level message formatter=
     fun value ->
         let formattedValue = formatter value
         log level $"%s{message}: %A{formattedValue}"
