[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.DependencyInjection)](https://www.nuget.org/packages/MinApiLib.DependencyInjection)

# MinApiLib.DependencyInjection

Register services in the DI container with an attribute.

## Installation

```bash
dotnet add package MinApiLib.DependencyInjection
```

## Usage

Register every decorated class of the calling assembly:

```csharp
global using MinApiLib.DependencyInjection;
// ...
builder.Services.AddAssembly();
// or, when they are in another assembly
builder.Services.AddAssembly(typeof(UserService).Assembly);
```

And decorate the services:

```csharp
[RegisterInIServiceCollection(typeof(IUserService), ServiceLifetime.Scoped)]
public class UserService : IUserService
{
    public string GetUserName() => "John Doe";
}
```

## Attribute forms

The default lifetime is `Transient`.

| Attribute | Registered as |
|---|---|
| `[RegisterInIServiceCollection]` | The class itself |
| `[RegisterInIServiceCollection(ServiceLifetime = ServiceLifetime.Scoped)]` | The class itself, with that lifetime |
| `[RegisterInIServiceCollection(typeof(IUserService))]` | The interface only; the class itself can't be resolved |
| `[RegisterInIServiceCollection(typeof(IUserService), ServiceLifetime.Scoped)]` | The interface only, with that lifetime |
| `[RegisterInIServiceCollection(typeof(IOne), typeof(ITwo), ServiceLifetime = ServiceLifetime.Singleton)]` | The class with that lifetime, and each interface forwarding to it |

With several types, each interface resolves the class registration, so a singleton is the same instance through all of them.

## Behavior

- Abstract classes and interfaces are ignored.
- Registrations are added, not replaced. If you add a service twice, the container resolves the last one.
- `AddAssembly` uses reflection, so it isn't suitable for native AOT.
