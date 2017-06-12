namespace R4nd0mApps.XTestPlatform.Implementation.XUnit

open R4nd0mApps.XTestPlatform.Api
open System.IO
open System.Reflection

module AdapterLoader = 
    let private loadDependencies() = 
        [ "xunit.abstractions.dll"; "xunit.runner.utility.net35.dll"; "R4nd0mApps.XTestPlatform.CecilUtils.dll" ]
        |> List.map (Prelude.tuple2 (Path.getLocalPath()) >> Path.Combine)
        |> List.iter (Assembly.LoadFrom >> ignore)
    
    let LoadDiscoverers : string -> seq<IXTestDiscoverer> = 
        fun _ -> 
            loadDependencies()
            [ XUnitTestDiscoverer() :> IXTestDiscoverer ] :> seq<_>
    
    let LoadExecutors : string -> seq<IXTestExecutor> = 
        fun _ -> 
            loadDependencies()
            [ XUnitTestExecutor() :> IXTestExecutor ] :> seq<_>
