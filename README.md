[![license](https://img.shields.io/badge/License-MIT-purple.svg)](../../LICENSE)
![This package is compatible with this framework or higher](https://img.shields.io/badge/.Net-7.0-blue)
![release action](https://github.com/fernandoescolar/MinApiLib/actions/workflows/release.yml/badge.svg)
![ci action](https://github.com/fernandoescolar/MinApiLib/actions/workflows/ci.yml/badge.svg)

# MinApiLib

Asp.Net Core >=7 Minimal Api Library with extensions, filters, and utilities to create easy cool APIs.

![MinApiLib](icon.png)

MinApiLib provides the following packages:


- [MinApiLib.DependencyInjection](src/MinApiLib.DependencyInjection/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.DependencyInjection)](https://www.nuget.org/packages/MinApiLib.DependencyInjection)
- [MinApiLib.Endpoints](src/MinApiLib.Endpoints/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.Endpoints)](https://www.nuget.org/packages/MinApiLib.Endpoints)
- [MinApiLib.HashedIds](src/MinApiLib.HashedIds/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.HashedIds)](https://www.nuget.org/packages/MinApiLib.HashedIds)
- [MinApiLib.Hypermedia](src/MinApiLib.Hypermedia/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.Hypermedia)](https://www.nuget.org/packages/MinApiLib.Hypermedia)
- [MinApiLib.Logging](src/MinApiLib.Logging/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.Logging)](https://www.nuget.org/packages/MinApiLib.Logging)
- [MinApiLib.OperationCanceled](src/MinApiLib.OperationCanceled/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.OperationCanceled)](https://www.nuget.org/packages/MinApiLib.OperationCanceled)
- [MinApiLib.Validation](src/MinApiLib.Validation/README.md) [![version](https://img.shields.io/nuget/vpre/MinApiLib.Validation)](https://www.nuget.org/packages/MinApiLib.Validation)

To install the packages, run the following command in the Package Manager Console:

```bash
Install-Package MinApiLib.<package-name>
```

Or using the .NET CLI:

```bash
dotnet add package MinApiLib.<package-name>
```

If you want to install all the packages, run the following command:

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

## License

[![license](https://img.shields.io/badge/License-MIT-purple.svg)](LICENSE)

The source code we develop at **MinApiLib** is default being licensed as **MIT**. You can read more about [here](LICENSE).

The package icon is the <a target="_blank" href="https://icons8.com/icon/1dMXEkMIykkN/constellation">Constellation</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>.
