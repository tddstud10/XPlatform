namespace R4nd0mApps.XTestPlatform.Implementation.XUnit

open R4nd0mApps.XTestPlatform.Api
open System
open Xunit
open Xunit.Abstractions

module Constants = 
    let extensionUri = "executor://xtestplatform/xUnit" |> Uri

module Converters = 
    open R4nd0mApps.XTestPlatform.CecilUtils
    open System.Collections.Generic
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
            use ndp = new NavigationDataProvider(src)
            let nd = ndp.GetNavigationData(tc.TestMethod.TestClass.Class.Name, tc.TestMethod.Method.Name)
            { TestCase = sfn tc
              Id = (Constants.extensionUri.ToString() + tc.UniqueID) |> guidFromString
              FullyQualifiedName = 
                  sprintf "%s.%s (%s)" tc.TestMethod.TestClass.Class.Name tc.TestMethod.Method.Name tc.UniqueID
              DisplayName = tc.DisplayName
              Source = src
              CodeFilePath = nd.FilePath
              LineNumber = nd.LineNumber
              ExtensionUri = Constants.extensionUri }
    
    type XTestResult with
        
        static member FromITestResultMessage (tcMap : IReadOnlyDictionary<_, _>) outcome (tr : ITestResultMessage) = 
            { DisplayName = tr.Test.DisplayName
              TestCase = tcMap.[tr.TestCase.UniqueID]
              Outcome = outcome
              FailureInfo = None }
        
        static member AddFailureInfo (msg : ITestFailed) (tr : XTestResult) = 
            { tr with FailureInfo = 
                          { Message = 
                                msg 
                                |> ExceptionUtility.CombineMessages
                                |> XTestErrorInfoParser.parseMessage
                            CallStack = 
                                msg
                                |> ExceptionUtility.CombineStackTraces
                                |> XTestErrorInfoParser.parseStackTrace }
                          |> Some }
