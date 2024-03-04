#load "runtime-scripts/Microsoft.AspNetCore.App-latest-8.fsx"
// to gen the above run this script in the local folder
// https://raw.githubusercontent.com/TheAngryByrd/IcedTasks/master/generate-sdk-references.fsx

#r "nuget: Feliz.ViewEngine"
#r "nuget: Microsoft.AspNetCore.OpenApi"
#r "nuget: Swashbuckle.AspNetCore"

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open System
open Feliz.ViewEngine
open Microsoft.AspNetCore.OpenApi
open Microsoft.Extensions.DependencyInjection
open System.Text
open Microsoft.AspNetCore.Http
open System.Threading.Tasks
open System.Net.Mime
open Microsoft.AspNetCore.Mvc
open System.Text.Json
open Microsoft.AspNetCore.Http.Json
open Swashbuckle.AspNetCore.SwaggerGen
open Microsoft.OpenApi.Models

//https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-8.0
//https://andrewlock.net/series/behind-the-scenes-of-minimal-apis/
//https://andrewlock.net/behind-the-scenes-of-minimal-apis-3-exploring-the-model-binding-logic-of-minimal-apis/

module API =

    let hello (x: int) (ctx: HttpContext) = task { return {| Bye = x |} }


module VIEW =
    let html s = s

    let dynamicList x =
        Html.ul
            [ for li in [ 1..x ] do
                  Html.li li ]
        |> Render.htmlView

    let mainView (str: string) =
        Html.html [ Html.h1 "HELLO"; Html.div str ] |> Render.htmlDocument

type HtmlResult(html) =
    interface IResult with
        member this.ExecuteAsync(httpContext) =
            httpContext.Response.ContentType <- MediaTypeNames.Text.Html
            httpContext.Response.ContentLength <- Encoding.UTF8.GetByteCount(s = html)
            httpContext.Response.WriteAsync(html)

    static member Create(html) = new HtmlResult(html)

// https://stackoverflow.com/questions/76876810/net-7-8-how-do-i-provide-a-description-summary-on-a-group-with-minimap-apis
type TagDescriptionsDocumentFilter() =
    interface IDocumentFilter with
        member this.Apply(swaggerDoc, _) =
            let newTag = new OpenApiTag(Name = "My API", Description = "My API does something")
            swaggerDoc.Tags <- new ResizeArray<OpenApiTag>()
            swaggerDoc.Tags.Add(newTag)



let builder = WebApplication.CreateSlimBuilder()

builder.Services.AddEndpointsApiExplorer()
builder.Services.AddSwaggerGen(fun opts -> opts.DocumentFilter<TagDescriptionsDocumentFilter>())

let app = builder.Build()

app.UseSwagger()
app.UseSwaggerUI()

app
    .MapGet("/", new Func<_>(fun () -> VIEW.mainView "HELLO" |> HtmlResult.Create))
    .ExcludeFromDescription()

app
    .MapGet("/2", new Func<_>(fun () -> VIEW.mainView "BYE" |> HtmlResult.Create))
    .ExcludeFromDescription()

app
    .MapGet(
        "/view/{id:int}",
        new Func<int, HttpContext, _>(fun id ctx ->
            task {
                //let x = ctx.Request.RouteValues["id"] :?> string |> int
                return VIEW.dynamicList id |> VIEW.mainView |> HtmlResult.Create
            })
    )
    .ExcludeFromDescription()

app
    .MapGet(
        "/api/hellos/{id:int}",
        new Func<int, HttpContext, _>(fun id ctx ->
            task {
                //let x = ctx.Request.RouteValues["id"] :?> string |> int

                return! API.hello id ctx
            })
    )
    .WithName("GetHellos")
    .WithOpenApi(fun o ->
        o.OperationId <- "GetHellos"
        o)
    .WithTags("API")

app
    .MapPost(
        "/api/hellos",
        Func<HttpContext, _, _>(fun ctx (reqBody: {| Hello: string; Bye: int |}) ->
            task {
                //let! hello = ctx.Request.ReadFromJsonAsync<{| Hello: string; Bye: int |}>()

                return! API.hello reqBody.Bye ctx
            })
    )
    .WithName("AddHello")
    .WithOpenApi(fun o ->
        o.OperationId <- "AddHello"
        o)
    .WithTags("API")

app.Run()
