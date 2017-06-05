namespace R4nd0mApps.XTestPlatform.Api

open System
open System.Runtime.Serialization
open System.Diagnostics
open Microsoft.FSharp.Reflection
open System.Reflection

// TODO: Split into differnt files
type XExtensionUri = Uri

[<KnownType("KnownTypes")>]
type XTestMessageLevel = 
    | Informational
    | Warning
    | Error
    static member KnownTypes() = 
        typeof<StackFrame>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic) 
        |> Array.filter FSharpType.IsUnion

[<CLIMutable>]
type XTestCase = 
    { TestCase : string
      CodeFilePath : string
      DisplayName : string
      ExtensionUri : XExtensionUri
      FullyQualifiedName : string
      Id : Guid
      LineNumber : int
      Source : string }

type IXMessageLogger = 
    abstract SendMessage : testMessageLevel:XTestMessageLevel * message:string -> unit

type IXTestCaseDiscoverySink = 
    abstract SendTestCase : discoveredTest:XTestCase -> unit

type IXTestDiscoverer = 
    abstract Id : string
    abstract ExtensionUri : XExtensionUri
    abstract DiscoverTests : sources:seq<string> * logger:IXMessageLogger * discoverySink:IXTestCaseDiscoverySink
     -> unit

[<KnownType("KnownTypes")>]
type XTestOutcome = 
    | None
    | Passed
    | Failed
    | Skipped
    | NotFound
    static member KnownTypes() = 
        typeof<StackFrame>.GetNestedTypes(BindingFlags.Public ||| BindingFlags.NonPublic) 
        |> Array.filter FSharpType.IsUnion

[<CLIMutable>]
type XTestResult = 
    { DisplayName : string
      EndTime : DateTimeOffset
      ErrorMessage : string
      ErrorStackTrace : string
      Outcome : XTestOutcome
      StartTime : DateTimeOffset
      TestCase : XTestCase }

type IXTestCaseExecutionSink = 
    inherit IXMessageLogger
    abstract RecordResult : testResult:XTestResult -> unit

type IXTestExecutor = 
    abstract Id : string
    abstract ExtensionUri : XExtensionUri
    abstract RunTests : tests:seq<XTestCase> * executionSink:IXTestCaseExecutionSink -> unit
    abstract Cancel : unit -> unit
