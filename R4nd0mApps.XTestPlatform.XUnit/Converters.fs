namespace R4nd0mApps.XTestPlatform.Implementation.XUnit

open R4nd0mApps.XTestPlatform.Api
open System
open Xunit
open Xunit.Abstractions

module Constants = 
    let extensionUri = "executor://xtestplatform/xUnit" |> Uri

module Converters = 
    open System.Security.Cryptography
    open System.Text
    
    let guidFromString (s : string) = 
        s
        |> Encoding.Unicode.GetBytes
        |> SHA1.Create().ComputeHash
        |> Array.take 16
        |> Guid
    
    type XTestCase with
        static member FromITestCase sfn src (tc : ITestCase) = 
            let srcInfo = tc.SourceInformation |> Option.ofNull
            { TestCase = sfn tc
              Id = (Constants.extensionUri.ToString() + tc.UniqueID) |> guidFromString
              FullyQualifiedName = 
                  sprintf "%s.%s (%s)" tc.TestMethod.TestClass.Class.Name tc.TestMethod.Method.Name tc.UniqueID
              DisplayName = tc.DisplayName
              Source = src
              CodeFilePath = srcInfo |> Option.fold (fun _ e -> e.FileName) null
              LineNumber = srcInfo |> Option.fold (fun _ e -> e.LineNumber.GetValueOrDefault(0)) 0
              ExtensionUri = Constants.extensionUri }
    
    type XTestResult with
        
        static member FromITestResultMessage sfn outcome (tr : ITestResultMessage) = 
            let tc = sfn tr.TestCase
            { DisplayName = tr.Test.DisplayName
              TestCase = tc
              Outcome = outcome
              ErrorStackTrace = null
              ErrorMessage = null }
        
        static member AddFailureInfo (msg : ITestFailed) (tr : XTestResult) = 
            { tr with ErrorMessage = msg |> ExceptionUtility.CombineMessages
                      ErrorStackTrace = msg |> ExceptionUtility.CombineStackTraces }
