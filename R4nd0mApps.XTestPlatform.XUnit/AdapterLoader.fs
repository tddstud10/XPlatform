namespace R4nd0mApps.XTestPlatform.Implementation.XUnit

open R4nd0mApps.XTestPlatform.Api

module AdapterLoader = 
    let LoadDiscoverers : string -> seq<IXTestDiscoverer> = Prelude.undefined
    let LoadExecutors : string -> seq<IXTestExecutor> = Prelude.undefined
