module R4nd0mApps.XTestPlatform.Api.DependencyLoader

open System
open System.IO
open System.Reflection

let registerDependencyResolver ds = 
    let resolveAssembly (ea : ResolveEventArgs) = 
        if ea.RequestingAssembly = Assembly.GetCallingAssembly() then 
            ds
            |> List.map (Path.GetFileNameWithoutExtension >> String.toLowerInvariant)
            |> List.tryFind ((=) (ea.Name |> String.toLowerInvariant))
            |> Option.fold (fun _ -> 
                   Prelude.tuple2 (Path.getLocalPath())
                   >> Path.Combine
                   >> Assembly.LoadFrom) null
        else null
    AppDomain.CurrentDomain.add_AssemblyResolve (ResolveEventHandler(Prelude.ct resolveAssembly))
