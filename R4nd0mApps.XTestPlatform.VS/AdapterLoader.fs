namespace R4nd0mApps.XTestPlatform.Implementation.VS

open R4nd0mApps.XTestPlatform.Api
open System
open System.Diagnostics
open System.IO
open System.Reflection

type AdapterInfo = 
    { Discoverer : string
      Executor : string }

type AdapterInfoMap = Map<string, AdapterInfo>

module AdapterLoader = 
    let private knownAdaptersMap : AdapterInfoMap = 
        [ ("xunit.runner.visualstudio.testadapter.dll".ToUpperInvariant(), 
           { Discoverer = "Xunit.Runner.VisualStudio.TestAdapter.VsTestRunner"
             Executor = "Xunit.Runner.VisualStudio.TestAdapter.VsTestRunner" })
          ("NUnit.VisualStudio.TestAdapter.dll".ToUpperInvariant(), 
           { Discoverer = "NUnit.VisualStudio.TestAdapter.NUnitTestDiscoverer"
             Executor = "NUnit.VisualStudio.TestAdapter.NUnitTestExecutor" })
          ("NUnit3.TestAdapter.dll".ToUpperInvariant(), 
           { Discoverer = "NUnit.VisualStudio.TestAdapter.NUnit3TestDiscoverer"
             Executor = "NUnit.VisualStudio.TestAdapter.NUnit3TestExecutor" }) ]
        |> Map.ofList
    
    let private createAdapter<'TIFace, 'TImpl> (adapterMap : AdapterInfoMap) selector path = 
        let loadAssembly = Assembly.LoadFrom
        
        let loadType a t = 
            let asm = loadAssembly a
            t
            |> asm.GetType
            |> Option.ofNull
        
        let asmResolver searchPath = 
            let innerFn _ (args : ResolveEventArgs) = 
                [ "*.dll"; "*.exe" ]
                |> Seq.map ((+) (AssemblyName(args.Name).Name))
                |> Seq.collect (fun name -> Directory.EnumerateFiles(searchPath, name, SearchOption.AllDirectories))
                |> Seq.tryFind File.Exists
                |> Option.fold (fun _ -> loadAssembly) null
            innerFn
        
        let resolver = asmResolver <| Path.GetDirectoryName path
        try 
            Trace.TraceInformation("Attempting to load Test Adapter {0} from {1}", typeof<'TIFace>, path)
            AppDomain.CurrentDomain.add_AssemblyResolve (ResolveEventHandler resolver)
            let createAdapter (t : Type) = 
                let inner = t |> Activator.CreateInstance
                let outer = (typeof<'TImpl>, [| inner |]) |> Activator.CreateInstance
                outer :?> 'TIFace
            
            let res = 
                adapterMap
                |> Map.tryFind (Path.GetFileName(path).ToUpperInvariant())
                |> Option.bind (fun ai -> loadType path (selector ai))
                |> Option.map createAdapter
            
            Trace.TraceInformation("Loaded Test Adapter {0} from {1}", typeof<'TIFace>, path)
            res
        finally
            AppDomain.CurrentDomain.remove_AssemblyResolve (ResolveEventHandler resolver)
    
    let private findAdapterAssemblies dir = 
        if Directory.Exists dir then Directory.EnumerateFiles(dir, "*.TestAdapter.dll", SearchOption.AllDirectories)
        else Seq.empty<string>
    
    let private loadDependencies _ = 
        [ "msdia120typelib_clr0200.dll"; "Microsoft.VisualStudio.TestPlatform.ObjectModel.dll" ]
        |> List.map (Prelude.tuple2 (Path.getLocalPath()) >> Path.Combine)
        |> List.iter (Assembly.LoadFrom >> ignore)
    
    let LoadDiscoverersWithMap adaptersMap = 
        Prelude.tee loadDependencies
        >> findAdapterAssemblies
        >> Seq.choose (createAdapter<IXTestDiscoverer, XTestDiscoverer> adaptersMap (fun x -> x.Discoverer))
    
    let LoadDiscoverers : string -> seq<IXTestDiscoverer> = fun x -> LoadDiscoverersWithMap knownAdaptersMap x
    
    let LoadExecutorsWithMap adaptersMap = 
        Prelude.tee loadDependencies
        >> findAdapterAssemblies
        >> Seq.choose (createAdapter<IXTestExecutor, XTestExecutor> adaptersMap (fun x -> x.Executor))
    
    let LoadExecutors : string -> seq<IXTestExecutor> = fun x -> LoadExecutorsWithMap knownAdaptersMap x
