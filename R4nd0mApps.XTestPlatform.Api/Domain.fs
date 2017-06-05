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
    abstract ExtensionUri : XExtensionUri
    abstract RunTests : tests:seq<XTestCase> * executionSink:IXTestCaseExecutionSink -> unit
    abstract Cancel : unit -> unit

module AdapterLoader = 
    open System.IO
    open System.Reflection
    
    let private invokeAPI<'T> apiName packagesPath = 
        let x =
            Assembly.GetExecutingAssembly().CodeBase
            |> Uri
            |> fun x -> x.LocalPath
            |> Path.GetDirectoryName
            |> Prelude.flip Prelude.tuple2 "R4nd0mApps.TestPlatform.VS.Implementation.dll"
            |> Path.Combine
        let y =
            x
            |> fun a -> Assembly.LoadFrom(a)
            |> fun a -> a.GetTypes()
        let z =
            y
            |> Seq.find (fun x -> x.Name = "AdapterLoader")
        let z1 =
            z
            |> fun t -> t.GetProperty(apiName, BindingFlags.Public ||| BindingFlags.Static)
        let z2 =
            z1
            |> fun p -> p.GetValue(null)  
        (z2 :?> (string -> 'T)) packagesPath
    
    let LoadDiscoverers p = invokeAPI<seq<IXTestDiscoverer>> "LoadDiscoverers" p
    let LoadExecutors p = invokeAPI<seq<IXTestExecutor>> "LoadExecutors" p
