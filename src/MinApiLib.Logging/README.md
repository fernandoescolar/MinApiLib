# MinApiLib.Logging

[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
[![version](https://img.shields.io/nuget/vpre/MinApiLib.Logging)](https://www.nuget.org/packages/MinApiLib.Logging)

This package contains extensions to use fast logging in your projects. It uses the best practices from [this Microsoft's article](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-7.0).

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

To use the logging, you can use the `ILogger` extensions:

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