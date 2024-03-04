#r "nuget: FSharp.Data"
#r "nuget: Fli"

open FSharp.Data
open System.IO
open System
open Fli

module AspNet =

    let genfile = __SOURCE_DIRECTORY__ + "/generate-sdk-references.fsx"

    let LoadLatest () =
        Http.RequestString(
            "https://raw.githubusercontent.com/TheAngryByrd/IcedTasks/master/generate-sdk-references.fsx"
        )
        |> fun c -> File.WriteAllText(genfile, c)


    let GetScripts () =
        cli {
            Shell Shells.BASH
            Command $"dotnet fsi {genfile}"
        }
        |> Command.execute


AspNet.LoadLatest()
AspNet.GetScripts()
