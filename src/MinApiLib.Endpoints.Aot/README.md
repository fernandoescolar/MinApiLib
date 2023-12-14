[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.Endpoints.Aot)](https://www.nuget.org/packages/MinApiLib.Endpoints.Aot)

# MinApiLib.Endpoints.Aot

This package provides extensions to use [MinApiLib.Endpoints](../MinApiLib.Endpoints/README.md) in AOT builds.
Table of conents:
- [MinApiLib.Endpoints.Aot](#minapilibendpointsaot)
  - [Installation](#installation)
  - [Usage](#usage)


## Installation

Firts, you need a project where you are using [MinApiLib.Endpoints](../MinApiLib.Endpoints/README.md). Then, you need to install this package.

You can install this package using either the NuGet package manager or the .NET CLI:

Using the NuGet package manager:

```bash
Install-Package MinApiLib.Endpoints.Aot
```

Using the .NET CLI:

```bash
dotnet add package MinApiLib.Endpoints.Aot
```

## Usage

To build the project in AOT mode, you need to replace the `Program.cs` file with the following code:

```csharp
using MinApiLib.Endpoints.Aot;

var builder = WebApplication.CreateBuilder(args);
// ...

var app = builder.Build();
// ...
// app.MapEndpoints(); // the non-AOT version
app.MapEndpointsAot(); // the AOT version
app.Run();
```

Them, you can build the project using the following command:

```bash
dotnet publish -c Release -r <RID> -p:PublishReadyToRun=true -p:PublishTrimmed=true
```

Where `<RID>` is the [runtime identifier](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) of the target platform.

And this project will generate the necesary code to run the endpoints in AOT mode.
