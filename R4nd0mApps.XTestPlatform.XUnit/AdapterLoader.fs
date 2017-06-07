namespace R4nd0mApps.XTestPlatform.Implementation.XUnit

open R4nd0mApps.XTestPlatform.Api

module AdapterLoader = 
    let LoadDiscoverers : string -> seq<IXTestDiscoverer> = 
        fun _ -> [XUnitTestDiscoverer() :> IXTestDiscoverer] :> seq<_>
    let LoadExecutors : string -> seq<IXTestExecutor> = 
        fun _ -> [XUnitTestExecutor() :> IXTestExecutor] :> seq<_>
