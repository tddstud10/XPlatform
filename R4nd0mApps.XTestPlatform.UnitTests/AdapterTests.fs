module R4nd0mApps.XTestPlatform.UnitTests

open FsUnit.Xunit
open R4nd0mApps.XTestPlatform.Api
open System
open System.Collections.Concurrent
open System.IO
open global.Xunit

let localPath = Path.getLocalPath()

let testBin = 
    localPath
    |> Prelude.flip Prelude.tuple2 "TestData/bins/XUnit20FSPortable/XUnit20FSPortable.dll"
    |> Path.Combine

let emptyTestCase = 
    { CodeFilePath = null
      DisplayName = null
      ExtensionUri = null
      FullyQualifiedName = null
      Id = Guid.Empty
      LineNumber = 0
      Source = null
      TestCase = null }

let expectedTests extensionUri = 
    [ { CodeFilePath = "D:\\src\\t\\TestProjects\\TestExecution\\TestData\\UnitTestProjects\\XUnit20FSPortable\\PortableLibrary1.fs"
        DisplayName = "XUnit20FSPortable.UnitTests.Fact Test 2"
        ExtensionUri = Uri(extensionUri)
        FullyQualifiedName = "XUnit20FSPortable.UnitTests.Fact Test 2 (f865f4f0ab6e9c84b5a7f916e99fe29768525d5e)"
        Id = Guid.Empty
        LineNumber = 17
        Source = "XUnit20FSPortable.dll"
        TestCase = null }
      { CodeFilePath = "D:\\src\\t\\TestProjects\\TestExecution\\TestData\\UnitTestProjects\\XUnit20FSPortable\\PortableLibrary1.fs"
        DisplayName = "XUnit20FSPortable.UnitTests.Fact Test 1"
        ExtensionUri = Uri(extensionUri)
        FullyQualifiedName = "XUnit20FSPortable.UnitTests.Fact Test 1 (9df5d496938dfc843c4799cfb7eadd84cf796382)"
        Id = Guid.Empty
        LineNumber = 13
        Source = "XUnit20FSPortable.dll"
        TestCase = null }
      { CodeFilePath = "D:\\src\\t\\TestProjects\\TestExecution\\TestData\\UnitTestProjects\\XUnit20FSPortable\\PortableLibrary1.fs"
        DisplayName = "XUnit20FSPortable.UnitTests.Theory Tests(input: 2)"
        ExtensionUri = Uri(extensionUri)
        FullyQualifiedName = "XUnit20FSPortable.UnitTests.Theory Tests (b8fd193983d2a702bf14e9659633be80f74ceda6)"
        Id = Guid.Empty
        LineNumber = 9
        Source = "XUnit20FSPortable.dll"
        TestCase = null }
      { CodeFilePath = "D:\\src\\t\\TestProjects\\TestExecution\\TestData\\UnitTestProjects\\XUnit20FSPortable\\PortableLibrary1.fs"
        DisplayName = "XUnit20FSPortable.UnitTests.Theory Tests(input: 1)"
        ExtensionUri = Uri(extensionUri)
        FullyQualifiedName = "XUnit20FSPortable.UnitTests.Theory Tests (a27d53f68c13b1f8dcee800f59f53efefd224fb3)"
        Id = Guid.Empty
        LineNumber = 9
        Source = "XUnit20FSPortable.dll"
        TestCase = null } ]
    |> List.sortBy (fun x -> x.DisplayName)

let normalizeTests ts = 
    ts |> Seq.map (fun t -> 
              { t with TestCase = null
                       Id = Guid.Empty
                       Source = Path.GetFileName(t.Source) })
let ``Test Data - Can discover tests`` : obj array seq = 
    [ "R4nd0mApps.XTestPlatform.XUnit.dll", "executor://xtestplatform/xUnit" ]
    @ if not Environment.IsMono then [ "R4nd0mApps.XTestPlatform.VS.dll", "executor://xunit/VsTestRunner2" ] else [ ]
    |> Seq.map (fun (a, b) -> [| box a; box b |])    

[<Theory>]
[<MemberData("Test Data - Can discover tests")>]
let ``Can discover tests`` (adapter, extensionUri) = 
    let discs = AdapterLoader.LoadDiscoverersFromPath [ (Path.Combine(localPath, adapter)) ] localPath
    let discoveredTests = ConcurrentBag<_>()
    discs |> Seq.iter (fun d -> 
                 d.TestDiscovered.Add(discoveredTests.Add)
                 d.DiscoverTests([ testBin ]))
    discoveredTests
    |> normalizeTests
    |> Seq.toList
    |> List.sortBy (fun x -> x.DisplayName)
    |> should equal (expectedTests extensionUri)
    discoveredTests |> Seq.iter (fun x -> x.Id |> should not' (equal Guid.Empty))
    discs
    |> Seq.map (fun d -> d.ExtensionUri.ToString())
    |> Seq.toList
    |> should equal [ extensionUri ]

let expectedTestResults = 
    [ { DisplayName = "XUnit20FSPortable.UnitTests.Fact Test 2"
        FailureInfo = 
            { Message = "Assert.Equal() Failure\r\nExpected: 1\r\nActual:   2" |> XErrorMessage
              CallStack = [| XErrorParsedFrame ((if not Environment.IsMono then "XUnit20FSPortable.UnitTests.Fact Test 2()" else "XUnit20FSPortable.UnitTests.Fact Test 2 ()"), 
                                                     "D:\\src\\t\\TestProjects\\TestExecution\\TestData\\UnitTestProjects\\XUnit20FSPortable\\PortableLibrary1.fs", 
                                                     17) |] |> XErrorStackTrace } |> Some
        Outcome = XTestOutcome.Failed
        TestCase = emptyTestCase }
      { DisplayName = "XUnit20FSPortable.UnitTests.Theory Tests(input: 1)"
        FailureInfo = None
        Outcome = XTestOutcome.Passed
        TestCase = emptyTestCase }
      { DisplayName = "XUnit20FSPortable.UnitTests.Theory Tests(input: 2)"
        FailureInfo = 
            { Message = "Assert.Equal() Failure\r\nExpected: 1\r\nActual:   2" |> XErrorMessage
              CallStack = [| XErrorParsedFrame ((if not Environment.IsMono then "XUnit20FSPortable.UnitTests.Theory Tests(Int32 input)" else "XUnit20FSPortable.UnitTests.Theory Tests (System.Int32 input)"), 
                                                      "D:\\src\\t\\TestProjects\\TestExecution\\TestData\\UnitTestProjects\\XUnit20FSPortable\\PortableLibrary1.fs", 
                                                      9) |] |> XErrorStackTrace } |> Some
        Outcome = XTestOutcome.Failed
        TestCase = emptyTestCase }
      { DisplayName = "XUnit20FSPortable.UnitTests.Fact Test 1"
        FailureInfo = None
        Outcome = XTestOutcome.Passed
        TestCase = emptyTestCase } ]
    |> List.sortBy (fun x -> x.DisplayName)

let normalizeTestResults = 
    let replace (s1: string) s2 (XErrorMessage m) =
        m.Replace(s1, s2) |> XErrorMessage
    let take n (XErrorStackTrace cs) =
        cs |> Array.take n |> XErrorStackTrace

    let normalizeFailureInfoForMono x =
        if not Environment.IsMono then x
        else { x with Message = x.Message |> replace "\n" "\r\n"; CallStack = x.CallStack |> take 1 }
    Seq.map (fun t -> 
            { t with 
                XTestResult.TestCase = emptyTestCase
                FailureInfo = t.FailureInfo |> Option.map normalizeFailureInfoForMono })

let stripSerializedTestCases x =
    { x with XTestCase.TestCase = null }

let ``Test Data - Can execute tests`` : obj array seq = 
    [ "R4nd0mApps.XTestPlatform.XUnit.dll", "executor://xtestplatform/xUnit" ]
    @ if not Environment.IsMono then [ "R4nd0mApps.XTestPlatform.VS.dll", "executor://xunit/VsTestRunner2" ] else [ ]
    |> Seq.map (fun (a, b) -> [| box a; box b |])    

[<Theory>]
[<MemberData("Test Data - Can execute tests")>]
let ``Can execute tests`` (adapter, extensionUri) = 
    let discoveredTests = ConcurrentBag<_>()
    AdapterLoader.LoadDiscoverersFromPath [ (Path.Combine(localPath, adapter)) ] localPath
    |> Seq.iter (fun d -> 
            d.TestDiscovered.Add(discoveredTests.Add)
            d.DiscoverTests([ testBin ]))
    let execs = AdapterLoader.LoadExecutorsFromPath [ (Path.Combine(localPath, adapter)) ] localPath
    let actualTestResults = ConcurrentBag<_>()
    execs 
    |> Seq.iter (fun e -> 
            e.TestExecuted.Add(actualTestResults.Add)
            e.RunTests(discoveredTests))
    execs
    |> Seq.map (fun e -> e.ExtensionUri.ToString())
    |> Seq.toList
    |> should equal [ extensionUri ]
    discoveredTests
    |> Seq.toList
    |> List.sortBy (fun x -> x.DisplayName)
    |> List.map stripSerializedTestCases
    |> should equal (actualTestResults |> Seq.toList |> List.sortBy (fun x -> x.DisplayName) |> List.map (fun x -> x.TestCase |> stripSerializedTestCases)) 
    actualTestResults
    |> normalizeTestResults
    |> Seq.toList
    |> List.sortBy (fun x -> x.DisplayName)
    |> should equal expectedTestResults
