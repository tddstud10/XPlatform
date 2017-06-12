namespace R4nd0mApps.XTestPlatform.Implementation.VS

open Microsoft.VisualStudio.TestPlatform.ObjectModel
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging
open R4nd0mApps.XTestPlatform.Api
open R4nd0mApps.XTestPlatform.Implementation.VS.Converters

type internal XTestDiscoverer(obj : obj) = 
    let vstd = obj :?> ITestDiscoverer
    
    let extensionUri = 
        obj.GetType().GetCustomAttributes(true)
        |> Seq.where (fun x -> x :? DefaultExecutorUriAttribute)
        |> Seq.cast<DefaultExecutorUriAttribute>
        |> Seq.head
        |> fun x -> x.ExecutorUri
        |> XExtensionUri

    let messageLogged = Event<_>()
    let testDiscovered = Event<_>()
    
    interface IXTestDiscoverer with
        member __.Id : string = obj.GetType().FullName
        member __.ExtensionUri : XExtensionUri = extensionUri
        member __.DiscoverTests(sources: seq<string>): unit = 
            vstd.DiscoverTests
                (sources, IDiscoveryContext.Create(), IMessageLogger.CreateMessageLogger messageLogged, 
                 ITestCaseDiscoverySink.Create testDiscovered)
        member __.Cancel(): unit = 
            failwith "Not implemented yet"
        member __.MessageLogged: IEvent<XTestMessageLevel * string> = messageLogged.Publish
        member __.TestDiscovered: IEvent<XTestCase> = testDiscovered.Publish

type internal XTestExecutor(obj : obj) = 
    let vste = obj :?> ITestExecutor
    
    let extensionUri = 
        obj.GetType().GetCustomAttributes(true)
        |> Seq.where (fun x -> x :? ExtensionUriAttribute)
        |> Seq.cast<ExtensionUriAttribute>
        |> Seq.head
        |> fun x -> x.ExtensionUri
        |> XExtensionUri

    let messageLogged = Event<_>()
    let testCompleted = Event<_>()
    
    interface IXTestExecutor with
        member __.Id : string = obj.GetType().FullName
        member __.ExtensionUri : XExtensionUri = extensionUri
        member __.RunTests(tests: seq<XTestCase>): unit = 
            let tests = tests |> Seq.map (fun x -> x.TestCase |> DataContract.deserialize<TestCase>)
            vste.RunTests(tests, IRunContext.CreateRunContext(), IFrameworkHandle.CreateFrameworkHandle messageLogged testCompleted)
        member __.Cancel() = vste.Cancel()
        member __.MessageLogged: IEvent<XTestMessageLevel * string> = messageLogged.Publish
        member __.TestExecuted: IEvent<XTestResult> = testCompleted.Publish
