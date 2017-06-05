namespace R4nd0mApps.XTestPlatform.Implementation.NUnit2

open R4nd0mApps.XTestPlatform.Api

type internal XTestDiscoverer() = 
    interface IXTestDiscoverer with
        member it.DiscoverTests(sources: seq<string>, logger: IXMessageLogger, discoverySink: IXTestCaseDiscoverySink): unit = 
            failwith "Not implemented yet"
        member it.ExtensionUri: XExtensionUri = 
            failwith "Not implemented yet"
        member it.Id: string = 
            failwith "Not implemented yet"

type internal XTestExecutor() = 
    interface IXTestExecutor with
        member it.Cancel(): unit = 
            failwith "Not implemented yet"
        member it.ExtensionUri: XExtensionUri = 
            failwith "Not implemented yet"
        member it.Id: string = 
            failwith "Not implemented yet"
        member it.RunTests(tests: seq<XTestCase>, executionSink: IXTestCaseExecutionSink): unit = 
            failwith "Not implemented yet"
