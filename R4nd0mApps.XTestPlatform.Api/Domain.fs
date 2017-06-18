namespace R4nd0mApps.XTestPlatform.Api

open Microsoft.FSharp.Reflection
open System
open System.Reflection
open System.Runtime.Serialization

type XExtensionUri = Uri

[<KnownType("KnownTypes")>]
type XTestMessageLevel = 
    | Informational
    | Warning
    | Error
    static member KnownTypes() = Serialization.knownTypes<XTestMessageLevel>()

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

[<KnownType("KnownTypes")>]
type XTestOutcome = 
    | NoOutcome
    | Passed
    | Failed
    | Skipped
    | NotFound
    static member KnownTypes() = Serialization.knownTypes<XTestOutcome>()

[<KnownType("KnownTypes")>]
type XErrorStackFrame = 
    | XErrorParsedFrame of string * string * int
    | XErrorUnparsedFrame of string
    static member KnownTypes() = Serialization.knownTypes<XErrorStackFrame>()

[<KnownType("KnownTypes")>]
type XErrorStackTrace = 
    | XErrorStackTrace of XErrorStackFrame[]
    static member KnownTypes() = Serialization.knownTypes<XErrorStackTrace>()

[<KnownType("KnownTypes")>]
type XErrorMessage = 
    | XErrorMessage of string
    static member KnownTypes() = Serialization.knownTypes<XErrorMessage>()

[<CLIMutable>]
type XTestFailureInfo = 
    { Message : XErrorMessage
      CallStack : XErrorStackTrace }

[<CLIMutable>]
type XTestResult = 
    { DisplayName : string
      FailureInfo : XTestFailureInfo option
      Outcome : XTestOutcome
      TestCase : XTestCase }

type IXTestDiscoverer = 
    abstract Id : string
    abstract ExtensionUri : XExtensionUri
    abstract DiscoverTests : sources:seq<string> -> unit
    abstract Cancel : unit -> unit
    abstract MessageLogged : IEvent<XTestMessageLevel * string>
    abstract TestDiscovered : IEvent<XTestCase>

type IXTestExecutor = 
    abstract Id : string
    abstract ExtensionUri : XExtensionUri
    abstract RunTests : tests:seq<XTestCase> -> unit
    abstract Cancel : unit -> unit
    abstract MessageLogged : IEvent<XTestMessageLevel * string>
    abstract TestExecuted  : IEvent<XTestResult>
