namespace R4nd0mApps.XTestPlatform.Api

open System
open System.Text.RegularExpressions

module XTestErrorInfoParser = 
    let private stackFrameCracker = 
        Regex
            ("""^at (?<method>(.*)) in (?<document>(.*))\:line (?<line>(\d+))$|^at (?<method>(.*)) \[0x[0-9a-f]*\] in (?<document>([^<>]*))\:(?<line>(\d+))$""", 
             RegexOptions.Compiled)
    let getValueOrDefault<'T> fconv = Option.ofNull >> Option.fold (Prelude.ct fconv) Unchecked.defaultof<'T>
    
    let parseStackTrace stackTrace = 
        let parseSF input = 
            let m = input |> stackFrameCracker.Match
            if m.Success then 
                (m.Groups.["method"].Value |> getValueOrDefault id, m.Groups.["document"].Value |> getValueOrDefault id, 
                 m.Groups.["line"].Value |> getValueOrDefault int) |> XErrorParsedFrame
            else input |> XErrorUnparsedFrame
        let cs = 
            if stackTrace |> String.IsNullOrWhiteSpace then [||]
            else 
                stackTrace.Split([| "\r\n"; "\r"; "\n" |], StringSplitOptions.RemoveEmptyEntries)
                |> Seq.map (String.trim >> parseSF)
                |> Seq.toArray
        cs |> XErrorStackTrace

    let parseMessage = 
        Option.ofNull 
        >> Option.getOrElse ""
        >> XErrorMessage
