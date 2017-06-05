// include Fake libs
#r "./packages/Build/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing
open System
open System.IO

MSBuildDefaults <- { MSBuildDefaults with Verbosity = Some MSBuildVerbosity.Minimal }
let assemblyVersion = EnvironmentHelper.environVarOrDefault "GitVersion_AssemblySemVer" "65535.65535.65535.65535"

// Directories
let packagesDir = __SOURCE_DIRECTORY__ @@ "packages" @@ "Build"
let buildDir  = __SOURCE_DIRECTORY__ @@ @"build"
let testDir  = __SOURCE_DIRECTORY__ @@ @"build"
let nugetDir = __SOURCE_DIRECTORY__ @@ @"NuGet"
ensureDirExists (directoryInfo nugetDir)

// Filesets
let solutionFile = "XTestPlatform.sln"

let msbuildProps = [
    "Configuration", "Debug"
    "Platform", "Any CPU"
]

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir]

    !! solutionFile
    |> MSBuild buildDir "Clean" msbuildProps
    |> ignore
)

Target "Rebuild" DoNothing

Target "Build" (fun _ ->
    !! solutionFile
    |> MSBuild buildDir "Build" msbuildProps
    |> ignore
)

Target "GitLink" (fun _ ->
    let gitLink = (packagesDir @@ @"gitlink" @@ "lib" @@ "net45" @@ "GitLink.exe")
    let args = sprintf "%s -f %s -d %s" __SOURCE_DIRECTORY__ solutionFile buildDir
    let ret =
        ExecProcessAndReturnMessages (fun info ->
            info.FileName <- gitLink
            info.Arguments <- args) (TimeSpan.FromSeconds 30.0)
    let consoleOutput =
        ret.Messages
        |> Seq.append ret.Errors
    consoleOutput
    |> Seq.iter (printfn "%s")
    let loadFailures =
        consoleOutput
        |> Seq.filter (fun m -> m.ToLowerInvariant().Contains("failed to load project"))
    if not ret.OK || not (Seq.isEmpty loadFailures) then failwith (sprintf "GitLink.exe \"%s\" task failed.\nErrors:\m %A" args loadFailures)
)

let runTest pattern =
    fun _ ->
        !! (buildDir @@  pattern)
        |> xUnit (fun p ->
            { p with
                ToolPath = findToolInSubPath "xunit.console.exe" (currentDirectory @@ "tools" @@ "xUnit")
                WorkingDir = Some testDir })

Target "Test" DoNothing
Target "UnitTests" (runTest "*.UnitTests*.dll")

Target "Package" (fun _ ->
    "XTestPlatform.nuspec"
    |> NuGet (fun p -> 
        { p with               
            Authors = [ "R4nd0m D3v3l0p3r" ]
            Project = "R4nd0mApps.XTestPlatform"
            Description = "R4nd0mApps XTestPlatform"
            Version = EnvironmentHelper.environVarOrDefault "GitVersion_NuGetVersion" "65535.65535.65535-alpha99"
            Dependencies = [ "FSharp.Core", ".." ]
                           |> List.map (fun (d,g) -> d, GetPackageVersion (packagesDir @@ g) d)
            OutputPath = buildDir })
)

Target "Publish" (fun _ ->
    !! "build/*.nupkg"
    |> AppVeyor.PushArtifacts
)

"Clean" ?=> "Build"
"Clean" ==> "Rebuild" 
"Build" ==> "Rebuild" 
"Build" ?=> "UnitTests" ==> "Test"
"Rebuild" ==> "Test"
"GitLink" ==> "Package"
"Test" ?=> "GitLink"
"Test" ==> "Package" ==> "Publish"

// start build
RunTargetOrDefault "Test"
