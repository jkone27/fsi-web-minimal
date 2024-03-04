#r "nuget: SwaggerProvider"
#r "nuget: FSharp.Data"

// https://fsprojects.github.io/SwaggerProvider/#/OpenApiClientProvider

open SwaggerProvider
open FSharp.Data
open System
open System.Text.RegularExpressions

module OpenApi =

    [<Literal>]
    let address = "http://127.0.0.1:5000/swagger/v1/swagger.yaml"

    //let replaceRegex = new Regex("\$ref: '(.+)'")

    let Update () =
        Http.RequestString(address)
        |> fun c -> c.Replace(@"<>f__AnonymousType", "")
        |> fun c -> System.IO.File.WriteAllText("openapi.yaml", c)


if fsi.CommandLineArgs.Length > 1 then
    if fsi.CommandLineArgs[1] = "--update-openapi" then
        OpenApi.Update()
        printfn "openapi.yaml was downloaded succesfully."
        exit 0



type TestApi = OpenApiClientProvider<"openapi.yaml">


let c = TestApi.Client()
c.HttpClient.BaseAddress <- "http://localhost:5000" |> Uri

let result = c.GetHellos(1).Result

printfn $"GET HELLOS: {result.Bye.Value}"

let req = new TestApi.Int32String3366518590()
req.Bye <- 51 |> Some
req.Hello <- "Hey"
let result2 = c.AddHello(req).Result

printfn $"ADD HELLO: {result2.Bye.Value}"
