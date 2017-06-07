namespace R4nd0mApps.XTestPlatform.Implementation.XUnit

open R4nd0mApps.XTestPlatform.Api
open System.Reflection
open System.IO

module AdapterLoader = 
    let private loadDependencies () = 
        [ "xunit.abstractions.dll"; "xunit.runner.utility.net35.dll" ]
        |> List.map (Prelude.tuple2 (Path.getLocalPath()) >> Path.Combine)
        |> List.iter (Assembly.LoadFrom >> ignore)

    let LoadDiscoverers : string -> seq<IXTestDiscoverer> = 
        fun _ -> 
            loadDependencies ()
            [XUnitTestDiscoverer() :> IXTestDiscoverer] :> seq<_>
    let LoadExecutors : string -> seq<IXTestExecutor> = 
        fun _ -> 
            loadDependencies ()
            [XUnitTestExecutor() :> IXTestExecutor] :> seq<_>
