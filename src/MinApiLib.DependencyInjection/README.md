[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.DependencyInjection)](https://www.nuget.org/packages/MinApiLib.DependencyInjection)

# MinApiLib.DependencyInjection

This package contains extensions to register services in the DI container using Attributes.

## Installation

You can install this package using the NuGet package manager:

```bash
Install-Package MinApiLib.DependencyInjection
```

Or using the .NET CLI:

```bash
dotnet add package MinApiLib.DependencyInjection
```

## Usage

To register a service, you can use the `RegisterInIServiceCollection` attribute:

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
