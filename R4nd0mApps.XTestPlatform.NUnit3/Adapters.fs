namespace R4nd0mApps.XTestPlatform.Implementation.NUnit3

open R4nd0mApps.XTestPlatform.Api

type internal XTestDiscoverer() = 
    interface IXTestDiscoverer with
        member it.ExtensionUri: XExtensionUri = 
            failwith "Not implemented yet"
        member it.Id: string = 
            failwith "Not implemented yet"
        member it.DiscoverTests(sources: seq<string>): unit = 
            failwith "Not implemented yet"
        member __.Cancel(): unit = 
            failwith "Not implemented yet"
        member it.MessageLogged: IEvent<XTestMessageLevel * string> = 
            failwith "Not implemented yet"
        member it.TestDiscovered: IEvent<XTestCase> = 
            failwith "Not implemented yet"

type internal XTestExecutor() = 
    interface IXTestExecutor with
        member it.ExtensionUri: XExtensionUri = 
            failwith "Not implemented yet"
        member it.Id: string = 
            failwith "Not implemented yet"
        member it.RunTests(tests: seq<XTestCase>): unit = 
            failwith "Not implemented yet"
        member it.MessageLogged: IEvent<XTestMessageLevel * string> = 
            failwith "Not implemented yet"
        member it.TestCompleted: IEvent<XTestResult> = 
            failwith "Not implemented yet"
        member it.Cancel(): unit = 
            failwith "Not implemented yet"
