# MinApiLib

Asp.Net Core >=7 Minimal Api Library with extensions, filters, and utilities to create easy cool APIs.

MinApiLib provides the following packages:

- [MinApiLib](#minapilib)
  - [MinApiLib.DependencyInjection](#minapilibdependencyinjection)
  - [MinApiLib.Endpoints](#minapilibendpoints)
  - [MinApiLib.HashedIds](#minapilibhashedids)
  - [MinApiLib.Hypermedia](#minapilibhypermedia)
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
dotnet add package MinApiLib.OperationCanceled
dotnet add package MinApiLib.Validation
```

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


## MinApiLib.HashedIds

## MinApiLib.Hypermedia

## MinApiLib.OperationCanceled


## MinApiLib.Validation
