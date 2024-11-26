module MediaManager.Features.Processing.Ffmpeg

open System.Diagnostics
open System
//TODO add cancellation token
let private StartInfo (arguments:string) =
    let sanitizedArgs = arguments.Replace("\\","/")
    let startInfo = ProcessStartInfo("ffmpeg.exe", $"-hide_banner -loglevel error -strict unofficial {sanitizedArgs}")
    startInfo.RedirectStandardError <- true
    startInfo.RedirectStandardOutput <- false
    startInfo.UseShellExecute <- false
    startInfo.CreateNoWindow <- true
    startInfo
    
let StartProcess (arguments:string) =
    use p = new Process()
    p.StartInfo <- StartInfo arguments
    p.Start() |> ignore
    p.WaitForExit()
    let errorOutput = p.StandardError.ReadToEnd()
    let noErrors = String.IsNullOrEmpty(errorOutput)
    noErrors && p.ExitCode = 0
