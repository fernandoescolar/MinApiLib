[![license](https://img.shields.io/badge/License-MIT-purple.svg)](LICENSE)
![version](https://img.shields.io/nuget/vpre/MinApiLib.Endpoints)

# MinApiLib

Asp.Net Core >=7 Minimal Api Library with extensions, filters, and utilities to create easy cool APIs.

MinApiLib provides the following packages:

- [MinApiLib](#minapilib)
  - [MinApiLib.DependencyInjection](#minapilibdependencyinjection)
  - [MinApiLib.Endpoints](#minapilibendpoints)
  - [MinApiLib.HashedIds](#minapilibhashedids)
  - [MinApiLib.Hypermedia](#minapilibhypermedia)
  - [MinApiLib.Logging](#minapiliblogging)
  - [MinApiLib.OperationCanceled](#minapiliboperationcanceled)
  - [MinApiLib.Validation](#minapilibvalidation)

To install the packages, run the following command in the Package Manager Console:

```bash
dotnet add package MinApiLib.<package-name>
```

Or if you want to install all the packages, run the following command:

```bash
dotnet add package MinApiLib.DependencyInjection
dotnet add package MinApiLib.Endpoints
dotnet add package MinApiLib.HashedIds
dotnet add package MinApiLib.Hypermedia
dotnet add package MinApiLib.Logging
dotnet add package MinApiLib.OperationCanceled
dotnet add package MinApiLib.Validation
```

Take a look at the [samples](samples) folder to see how to use the packages.

## MinApiLib.DependencyInjection

This package contains extensions to register services in the DI container using Attributes:

```csharp
[RegisterInIServiceCollection]
public class UserService : IUserService
{
    public string GetUserName() => "John Doe";
}
```

By default, the service is registered as transient and for the same type of the service, but you can change this behavior by adding the `Type` and / or the `ServiceLifetime` parameters:

```csharp
[RegisterInIServiceCollection(typeof(IUserService), ServiceLifetime.Scoped)] )]
public class UserService : IUserService
{
    public string GetUserName() => "John Doe";
}
```

To register all services in the assembly, you can use the `AddAssembly` method:

```csharp
builder.Services.AddAssembly();
```

By default, it wil register the classes in the current executing assembly, but you can change this behavior by adding the `Assembly` parameter:

```csharp
builder.Services.AddAssembly(typeof(SomeObject).Assembly);
```

## MinApiLib.Endpoints

This package contains extensions to create endpoints using records and one file per endpoint.

`MapEndpoints` extension method to map all endpoints in the assembly:

```csharp
global using MinApiLib.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapEndpoints();
app.Run();
```

By default, it wil map the classes in the current executing assembly, but you can change this behavior by adding the `Assembly` parameter:

```csharp
app.MapEndpoints(typeof(SomeObject).Assembly);
```

Then, you can create endpoints:

```csharp
public record Hello() : GetEndpoint("/hello")
{
    public IResult Handle()
    {
        return Results.Ok("Hello World!");
    }
}
```

And test it with curl:

```bash
$ curl "http://localhost:5000/hello"
"Hello World!"%
```

This endpoint accepts asynchrony and cancellation:

```csharp
public record Hello() : GetEndpoint("/hello")
{
    public async Task<IResult> HandleAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        return Results.Ok("Hello World!");
    }
}
```

This endpoint accepts a request object:

```csharp
public record struct Request(string Name, CancellationToken CancellationToken);

public record Hello() : GetEndpoint<Request>("/hello")
{
    protected override async Task<IResult> OnHandleAsync(Request request, CancellationToken CancellationToken)
    {
        await Task.Delay(1000, request.CancellationToken);
        return Results.Ok($"Hello {request.Name}!");
    }
}
```

You can test it with curl:

```bash
$ curl "http://localhost:5000/hello?name=fer"
"Hello fer!"%
```

And you can decorate the request object with attributes:

```csharp
public record struct Request([FromQuery] string Name, CancellationToken CancellationToken);
```

This endpoint accepts also a response object:

```csharp
public record struct Request(string Name, CancellationToken CancellationToken);

public record struct Response(string Message);

public record Hello() : GetEndpoint<Request, Response>("/hello")
{
    protected override async Task<Response> OnHandleAsync(Request request)
    {
        await Task.Delay(1000, request.CancellationToken);
        var response = new Response($"Hello {request.Name}!");
        return Results.Ok(response);
    }
}
```

You can test it with curl:

```bash
$ curl "http://localhost:5000/hello?name=fer"
{"message":"Hello fer!"}%
```

And you can configure any endpoint overriding the `OnConfigure` method:

```csharp
public record Hello() : GetEndpoint("/hello")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .WithName("Hello")
                .WithTags("hello", "world");

    public IResult Handle()
    {
        return Results.Ok("Hello World!");
    }
}
```


Here is a list of all the available endpoints:

- `GetEndpoint`
- `PostEndpoint`
- `PutEndpoint`
- `DeleteEndpoint`
- `PatchEndpoint`

## MinApiLib.HashedIds

This package contains extensions to use hashed ids in your endpoints. It uses the [Hashids](https://hashids.org/) library. You can configure the options by adding the `Passphrase` parameter:

```csharp
global using MinApiLib.HashedIds;
// ...
builder.Services.AddHashedIds(op => op.Passphrase = "my secret passphrase");
```

Then, you can use the `HashedId` type in your endpoints:

```csharp
public record struct Response(HashedId Id, string Name);

public record Get() : GetEndpoint("/things")
{
    public IResult Handle()
        => Results.Ok(new Response(1, "John Doe"));
}
```

And test it with curl:

```bash
$ curl "http://localhost:5000/things"
{"id":"Wd","name":"John Doe"}%
```

You can also use the `HashedId` type in your requests and responses:

```csharp
public record struct Request(HashedId Id, CancellationToken CancellationToken);

public record struct Response(HashedId Id, string Name);

public record GetThing() : GetEndpoint<Request>("/things/{id}")
{
    protected override Task<IResult> OnHandleAsync(Request request, CancellationToken cancellationToken)
    {
        int id = request.Id;
        var response = new Response(id, "John Doe");
        return Task.FromResult(Results.Ok(response));
    }
}
```

And test it:

```bash
$ curl "http://localhost:5000/things/Wd"
{"id":"Wd","name":"John Doe"}%
```

## MinApiLib.Hypermedia

To use hypermedia, you need to add the following code in your `Program.cs`:

```csharp
global using MinApiLib.Hypermedia;
// ...
builder.Services.AddHypermedia();
// ...
app.MapEndpoints()
   .WithHypermedia();
```

It will use *"application/vnd.hypermedia+json"* as default mime type, but you can change it by adding the `contentType` parameter:

```csharp
builder.Services.AddHypermedia(contentType: "application/vnd.my.format+json");
```

Then you should create your hypermedia providers for your response models:

```csharp
public record struct Response(HashedId Id, string Name);

public class ResponseHypermediaProvider : HypermediaProvider<Response>
{
    protected override IEnumerable<HypermediaLink> GetLinksFor(Response @object)
    {
        yield return new HypermediaLink("self", "/things/" + @object.Id, "GET");
        yield return new HypermediaLink("update", "/things/" + @object.Id, "PUT");
        yield return new HypermediaLink("delete", "/things/" + @object.Id, "DELETE");
        yield return new HypermediaLink("beers", "/things", "GET");
    }
}

public record GetThing() : GetEndpoint("/things/{id}")
{
    public IResult Handle(int id)
        => Results.Ok(new Response(id, "John Doe"));
}
```

You should add "application/vnd.hypermedia+json" as "Accept" header in order to get hypermedia links:

```bash
$ curl -H "Accept: application/vnd.hypermedia+json" "http://localhost:5000/things/1"
{"value":{"id":"Wk","name":"John Doe"},"links":[{"href":"http://localhost:5000/things/Wk","rel":"self","method":"GET"},{"href":"http://localhost:5000/things/Wk","rel":"update","method":"PUT"},{"href":"http://localhost:5000/things/Wk","rel":"delete","method":"DELETE"},{"href":"http://localhost:5000/things","rel":"beers","method":"GET"}]}%
```

## MinApiLib.Logging

This package contains extensions to use fast logging in your projects. It uses the best practices from [this Microsoft's article](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-7.0).

```csharp
global using MinApiLib.Logging;
```

This `ILogger` extensions will add the following methods:

```csharp
ILogger.Critical(string message, Exception? ex);
ILogger.Debug(string message);
ILogger.Error(string message, Exception? ex);
ILogger.Information(string message);
ILogger.Trace(string message);
```

## MinApiLib.OperationCanceled

This package contains extensions to handle `OperationCanceledException` in your endpoints and creates 499 status code response.

```csharp
global using MinApiLib.OperationCanceled;
// ...
app.CatchOperationCanceled();
```


## MinApiLib.Validation

This package contains extensions to use data annotations in your endpoints. It uses the [MiniValidation](https://github.com/DamianEdwards/MiniValidation) library.

```csharp
global using MinApiLib.Validation;
```

Now configure the validation filter in your endpoint:

```csharp
public record struct Request(
    [FromBody] RequestBody Body
);

public class RequestBody
{
    [Required, StringLength(150, MinimumLength = 3)]
    public string Name { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string Country { get; set; }
}

public record CreateThing() : PostEndpoint<Request>("things")
{
    protected override void OnConfigure(RouteHandlerBuilder builder)
        => builder
                .Produces<Response>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithName("CreateThings")
                .WithTags("things")
                .WithValidation();

    protected override async Task<IResult> OnHandleAsync(Request request, CancellationToken cancellationToken)
    {
        // async stuff
        return Results.Created($"/things/{created.Id}", created);
    }
}
```

It will validate the request body and return a 400 status code response if the validation fails:

```bash
$ curl -X POST -H "Content-Type: application/json" -d '{"name":"John Doe"}' "http://localhost:5000/things"
{"City":["The City field is required."],"Country":["The Country field is required."]}%
```