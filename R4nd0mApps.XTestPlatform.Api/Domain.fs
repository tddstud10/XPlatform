namespace R4nd0mApps.XTestPlatform.Api

open Microsoft.FSharp.Reflection
open System
open System.Diagnostics
open System.Reflection
open System.Runtime.Serialization

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
      ErrorMessage : string
      ErrorStackTrace : string
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
    abstract TestCompleted  : IEvent<XTestResult>
