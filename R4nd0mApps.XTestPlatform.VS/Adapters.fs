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
    
    interface IXTestDiscoverer with
        member __.Id : string = obj.GetType().FullName
        member __.ExtensionUri : XExtensionUri = extensionUri
        member __.DiscoverTests(sources, logger, discoverySink) = 
            vstd.DiscoverTests
                (sources, IDiscoveryContext.Create(), IMessageLogger.Create logger, 
                 ITestCaseDiscoverySink.Create discoverySink)

type internal XTestExecutor(obj : obj) = 
    let vste = obj :?> ITestExecutor
    
    let extensionUri = 
        obj.GetType().GetCustomAttributes(true)
        |> Seq.where (fun x -> x :? ExtensionUriAttribute)
        |> Seq.cast<ExtensionUriAttribute>
        |> Seq.head
        |> fun x -> x.ExtensionUri
        |> XExtensionUri
    
    interface IXTestExecutor with
        member __.Id : string = obj.GetType().FullName
        member __.ExtensionUri : XExtensionUri = extensionUri
        member __.Cancel() = vste.Cancel()
        member __.RunTests(tests : seq<XTestCase>, executionSink : IXTestCaseExecutionSink) = 
            let tests = tests |> Seq.map (fun x -> x.TestCase |> DataContract.deserialize<TestCase>)
            vste.RunTests(tests, IRunContext.CreateRunContext(), IFrameworkHandle.Create executionSink)
