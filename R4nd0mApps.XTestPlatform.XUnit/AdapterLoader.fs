namespace R4nd0mApps.XTestPlatform.Implementation.XUnit

open R4nd0mApps.XTestPlatform.Api

module AdapterLoader = 
    let private deps = 
        [ "xunit.abstractions.dll"; "xunit.runner.utility.net35.dll"; "R4nd0mApps.XTestPlatform.CecilUtils.dll" ]
    
    let LoadDiscoverers : string -> seq<IXTestDiscoverer> = 
        fun _ -> 
            DependencyLoader.registerDependencyResolver (Path.getLocalPath()) deps
            [ XUnitTestDiscoverer() :> IXTestDiscoverer ] :> seq<_>
    
    let LoadExecutors : string -> seq<IXTestExecutor> = 
        fun _ -> 
            DependencyLoader.registerDependencyResolver (Path.getLocalPath()) deps
            [ XUnitTestExecutor() :> IXTestExecutor ] :> seq<_>
