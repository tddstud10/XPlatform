module R4nd0mApps.XTestPlatform.Api.DependencyLoader

open System
open System.IO
open System.Reflection

let registerDependencyResolver searchPath deps = 
    let resolveAssembly (ea : ResolveEventArgs) = 
        deps
        |> List.tryFind (Path.GetFileNameWithoutExtension >> String.equalsOrdinalCI ea.Name)
        |> Option.fold (fun _ -> (Prelude.tuple2 searchPath >> Path.Combine) >> Assembly.LoadFrom) null
    AppDomain.CurrentDomain.add_AssemblyResolve (ResolveEventHandler(Prelude.ct resolveAssembly))
