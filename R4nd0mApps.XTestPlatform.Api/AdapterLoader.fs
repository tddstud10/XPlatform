namespace R4nd0mApps.XTestPlatform.Api

open System

module AdapterLoader = 
    open System.IO
    open System.Reflection
    
    let private invokeAPI<'T> apiName packagesPath = 
        Assembly.GetExecutingAssembly().CodeBase
        |> Uri
        |> fun x -> x.LocalPath
        |> Path.GetDirectoryName
        |> Prelude.flip Prelude.tuple2 "R4nd0mApps.TestPlatform.VS.Implementation.dll"
        |> Path.Combine
        |> fun a -> Assembly.LoadFrom(a)
        |> fun a -> a.GetTypes()
        |> Seq.find (fun x -> x.Name = "AdapterLoader")
        |> fun t -> t.GetProperty(apiName, BindingFlags.Public ||| BindingFlags.Static)
        |> fun p -> (p.GetValue(null) :?> string -> 'T) packagesPath
    
    let LoadDiscoverers p = invokeAPI<seq<IXTestDiscoverer>> "LoadDiscoverers" p
    let LoadExecutors p = invokeAPI<seq<IXTestExecutor>> "LoadExecutors" p
