# MinApiLib.OperationCanceled

[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.OperationCanceled)](https://www.nuget.org/packages/MinApiLib.OperationCanceled)

This package contains extensions to handle `OperationCanceledException` in your endpoints and creates `499` status code response.

## Installation

You can install this package using the NuGet package manager:

```bash
Install-Package MinApiLib.OperationCanceled
```

Or using the .NET CLI:

```bash
dotnet add package MinApiLib.OperationCanceled
```

## Usage

To use the `OperationCanceledException` handler, you can use the `CatchOperationCanceled` extension method:

```csharp
global using MinApiLib.OperationCanceled;
// ...
app.CatchOperationCanceled();
```
