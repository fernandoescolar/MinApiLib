using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;

namespace MinApiLib.AspNetCore.CodeGenerator;

public class MinApiLibCommandLineGeneratorModel
{
    [Option(Name = "model", ShortName = "m", Description = "Model class to use")]
    public string ModelClass { get; set; }

    [Option(Name = "dataContext", ShortName = "dc", Description = "DbContext class to use")]
    public string DataContextClass { get; set; }

    [Option(Name = "relativeFolderPath", ShortName = "outDir", Description = "Specify the relative output folder path from project where the file needs to be generated, if not specified, file will be generated in the project folder")]
    public string RelativeFolderPath { get; set; }

    [Option(Name = "databaseProvider", ShortName = "dbProvider", Description = "Database provider to use. Options include 'sqlserver' (default), 'sqlite', 'cosmos', 'postgres'.")]
    public string DatabaseProviderString { get; set; }
    public DbProvider DatabaseProvider { get; set; }

    public MinApiLibCommandLineGeneratorModel()
    {
    }

    protected MinApiLibCommandLineGeneratorModel(MinApiLibCommandLineGeneratorModel copyFrom)
    {
        ModelClass = copyFrom.ModelClass;
        DataContextClass = copyFrom.DataContextClass;
    }

    public MinApiLibCommandLineGeneratorModel Clone()
    {
        return new MinApiLibCommandLineGeneratorModel(this);
    }
}

public static class MinApiLibCommandLineGeneratorModelExtensions
{
    public static void ValidateCommandline(this MinApiLibCommandLineGeneratorModel model, ILogger logger)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        if (!string.IsNullOrEmpty(model.DatabaseProviderString) && EfConstants.AllDbProviders.TryGetValue(model.DatabaseProviderString, out var dbProvider))
        {
            model.DatabaseProvider = dbProvider;
        }
    }
}