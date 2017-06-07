module R4nd0mApps.XTestPlatform.Implementation.VS.Converters

open Microsoft.VisualStudio.TestPlatform.ObjectModel
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging
open R4nd0mApps.XTestPlatform.Api
open System
open System.Collections.Generic

type IRunSettings with
    static member Create() = 
        { new IRunSettings with
              member __.GetSettings(_ : string) : ISettingsProvider = failwith "Not implemented yet"
              member __.SettingsXml : string = "<RunSettings/>" }

type IDiscoveryContext with
    static member Create() = 
        { new IDiscoveryContext with
              member __.RunSettings : IRunSettings = IRunSettings.Create() }

type XTestCase with
    static member Create(x : TestCase) = 
        { TestCase = DataContract.serialize x
          Id = x.Id
          FullyQualifiedName = x.FullyQualifiedName
          DisplayName = x.DisplayName
          Source = x.Source
          CodeFilePath = x.CodeFilePath
          LineNumber = x.LineNumber
          ExtensionUri = x.ExecutorUri }

type ITestCaseDiscoverySink with
    static member Create(testDiscovered : Event<_>) = 
        { new ITestCaseDiscoverySink with
              member __.SendTestCase(discoveredTest : TestCase) : unit = testDiscovered.Trigger(XTestCase.Create discoveredTest) }

type XTestMessageLevel with
    static member Create(x : TestMessageLevel) = 
        match x with
        | TestMessageLevel.Informational -> XTestMessageLevel.Informational
        | TestMessageLevel.Warning -> XTestMessageLevel.Warning
        | TestMessageLevel.Error -> XTestMessageLevel.Error
        | _ -> Prelude.undefined

type IMessageLogger with
    static member CreateMessageLogger(messageLogged : Event<_>) = 
        { new IMessageLogger with
              member __.SendMessage(testMessageLevel : TestMessageLevel, message : string) : unit = 
                  messageLogged.Trigger(XTestMessageLevel.Create testMessageLevel, message) }

type IRunContext with
    static member CreateRunContext() = 
        { new IRunContext with
              member __.GetTestCaseFilter(_ : IEnumerable<string>, _ : Func<string, TestProperty>) : ITestCaseFilterExpression = 
                  null
              member __.InIsolation : bool = false
              member __.IsBeingDebugged : bool = false
              member __.IsDataCollectionEnabled : bool = false
              member __.KeepAlive : bool = false
              member __.RunSettings : IRunSettings = IRunSettings.Create()
              member __.SolutionDirectory : string = null
              member __.TestRunDirectory : string = null }

type XTestOutcome with
    static member Create = 
        function 
        | TestOutcome.None -> XTestOutcome.None
        | TestOutcome.Passed -> XTestOutcome.Passed
        | TestOutcome.Failed -> XTestOutcome.Failed
        | TestOutcome.Skipped -> XTestOutcome.Skipped
        | TestOutcome.NotFound -> XTestOutcome.NotFound
        | _ -> Prelude.undefined

type XTestResult with
    static member Create(x : TestResult) = 
        { DisplayName = x.DisplayName
          TestCase = XTestCase.Create x.TestCase
          Outcome = XTestOutcome.Create x.Outcome
          ErrorStackTrace = x.ErrorStackTrace
          ErrorMessage = x.ErrorMessage }

type IFrameworkHandle with
    static member CreateFrameworkHandle (msgLogged : Event<_>) (testCompleted : Event<_>) = 
        { new IFrameworkHandle with
              
              member __.EnableShutdownAfterTestRun 
                  with get () = true : bool
                  and set (_ : bool) = () : unit
              
              member __.LaunchProcessWithDebuggerAttached(_ : string, _ : string, _ : string, 
                                                          _ : IDictionary<string, string>) : int = 0
              member __.RecordAttachments(_ : IList<AttachmentSet>) : unit = ()
              member __.RecordEnd(_ : TestCase, _ : TestOutcome) : unit = ()
              
              member __.RecordResult(testResult : TestResult) : unit = 
                  testResult
                  |> XTestResult.Create
                  |> testCompleted.Trigger
              
              member __.RecordStart(_ : TestCase) : unit = ()
              member __.SendMessage(testMessageLevel : TestMessageLevel, message : string) : unit = 
                  msgLogged.Trigger(XTestMessageLevel.Create testMessageLevel, message) }
