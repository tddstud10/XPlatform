module R4nd0mApps.XTestPlatform.Implementation.VS.AdapterLoaderTests

open global.Xunit
open R4nd0mApps.XTestPlatform.Implementation.VS
open System.IO
open FsUnit.Xunit

let adapterSearchPath = Path.getLocalPath()

let testDataRoot = 
    Path.combine [ adapterSearchPath; "TestData" ]

let adapterMap = 
    [ ("aunit.testadapter.dll".ToUpperInvariant(), 
       { Discoverer = "AUnit.TestAdapter.VsDiscoverer"
         Executor = "AUnit.TestAdapter.VsTestExecutor" })
      ("bUnit.Testadapter.dll".ToUpperInvariant(), 
       { Discoverer = "BUnit.TestAdapter.VsTestDiscoverer"
         Executor = "BUnit.TestAdapter.VsTestExecutor" }) ]
    |> Map.ofList

let ``Test Data for Nested search path with no adapters, return empty`` : obj array seq = 
    [| AdapterLoader.LoadDiscoverersWithMap adapterMap >> Seq.map box
       AdapterLoader.LoadExecutorsWithMap adapterMap >> Seq.map box |]
    |> Seq.map (fun a -> [| box a |])

[<Theory>]
[<MemberData("Test Data for Nested search path with no adapters, return empty")>]
let ``Non existant path, return empty`` (f : string -> obj seq) = 
    let sp = 
        Path.combine [ testDataRoot
                       "NonExistantPath" ]
    Assert.Empty(sp |> f)

[<Theory>]
[<MemberData("Test Data for Nested search path with no adapters, return empty")>]
let ``Nested search path with no adapters, return empty`` (f : string -> obj seq) = 
    let sp = 
        Path.combine [ testDataRoot
                       "NoTestAdapters" ]
    Assert.NotEmpty(Directory.EnumerateFiles(sp, "*.*", SearchOption.AllDirectories))
    Assert.Empty(sp |> f)

let ``Test Data for Nested Search path with 2 adapters, return both`` : obj array seq = 
    [| (AdapterLoader.LoadDiscoverersWithMap adapterMap >> Seq.map (fun x -> x.Id), 
        [ "AUnit.TestAdapter.VsDiscoverer"; "BUnit.TestAdapter.VsTestDiscoverer" ])
       
       (AdapterLoader.LoadExecutorsWithMap adapterMap >> Seq.map (fun x -> x.Id), 
        [ "AUnit.TestAdapter.VsTestExecutor"; "BUnit.TestAdapter.VsTestExecutor" ]) |]
    |> Seq.map (fun (a, b) -> 
           [| box a
              box b |])

[<Theory>]
[<MemberData("Test Data for Nested Search path with 2 adapters, return both")>]
let ``Nested Search path with 2 adapters, return both`` (f : string -> string seq, adapters : string list) = 
    let sp = 
        Path.combine [ testDataRoot
                       "TestAdapters" ]
    let found = 
        sp
        |> f
        |> Seq.sort
        |> List.ofSeq
    
    found |> should equal adapters