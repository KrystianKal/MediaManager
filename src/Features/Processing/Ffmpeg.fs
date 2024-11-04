module MediaManager.Features.Processing.Ffmpeg

open System.Diagnostics
open System
//TODO add cancellation token
let private StartInfo (arguments:string) =
    let startInfo = ProcessStartInfo("ffmpeg.exe", $"-hide_banner -loglevel error {arguments}")
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
    if not (String.IsNullOrEmpty(errorOutput)) then
        printfn $"FFmpeg Error: %s{errorOutput}"
