# F# interactive minimal web api template (.fsx) 🦔

![alt text](image.png)

The `fsi-web-minimal` .NET template can be used to generate F# minimal API apps using only .fsx scripts, without a full `.fsproj`, but only with `.fsx` files.

## installation

```
cd working/content
dotnet new install .
dotnet new fsi-web-minimal
```

## Run

run the download script
`dotnet fsi downloadScripts.fsx`

then run 
`dotnet fsi generate-sdk-references.fsx`

then to run the app make sure you have installed NET8 SDK LTS,
then run to start the server, listening to port 5000.
`dotnet fsi script.fsx`

## OpenApi

navigate to `http://localhost/swagger` to check the OpenApi spec

## Testing 

to live test the api, download the openapi spec while your app is running, for convenience the `test.fsx` script includes also such utility function, remember to run this script your server must be running and serving the openapi/swagger metadata, it won't work if you didn't start your script yet.

`dotnet fsi test.fsx --update-openapi`

then normally run the test `dotnet fsi test.fsx` 

the test is performed against the OpenApi contract using the awesome [SwaggerProvider](https://fsprojects.github.io/SwaggerProvider/#/)
you should see in your server terminal window the endpoints being hit, and you will get a print result for your test endpoint invocation.

### DevContainer and DOCKER

TBD, is possible obviously to run these commands in DOCKER just using the microsoft dotnet-sdk image, that also includes `dotnet fsi` as command.

### Why

the convenience of a single script makes possibly F# a great language for true microservices, ideally all isolated and testable with their own public openapi specification, a monorepo or many repos you can decide for your own. 

