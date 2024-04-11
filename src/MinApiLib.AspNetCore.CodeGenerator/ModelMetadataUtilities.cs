using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc;

namespace MinApiLib.AspNetCore.CodeGenerator;

internal static class ModelMetadataUtilities
{
    internal static async Task<ModelTypeAndContextModel> GetModelEFMetadataMinApiLibAsync(
        MinApiLibCommandLineGeneratorModel commandLineModel,
        IEntityFrameworkService entityFrameworkService,
        IModelTypesLocator modelTypesLocator,
        ILogger logger,
        string areaName)
    {
        ModelType model = ValidationUtil.ValidateType(commandLineModel.ModelClass, "model", modelTypesLocator);
        // Validation successful
        Contract.Assert(model != null, "ValidationSuccessfull_modelUnset");

        ModelType dataContext = null;
        var dbContextFullName = string.Empty;
        ContextProcessingResult modelMetadata  = new ContextProcessingResult()
        {
            ContextProcessingStatus = ContextProcessingStatus.MissingContext,
            ModelMetadata = null
        };

        if (!string.IsNullOrEmpty(commandLineModel.DataContextClass))
        {
            dataContext = ValidationUtil.ValidateType(commandLineModel.DataContextClass, "dataContext", modelTypesLocator, throwWhenNotFound: false);
            dbContextFullName = dataContext != null ? dataContext.FullName : commandLineModel.DataContextClass;
            if (dataContext == null)
            {
                commandLineModel.DatabaseProvider = ValidateDatabaseProvider(commandLineModel.DatabaseProviderString, logger);
            }

            modelMetadata = await entityFrameworkService.GetModelMetadata(
                dbContextFullName,
                model,
                areaName,
                databaseProvider: commandLineModel.DatabaseProvider);
        }

        return new ModelTypeAndContextModel()
        {
            ModelType = model,
            DbContextFullName = dbContextFullName,
            ContextProcessingResult = modelMetadata,
            DatabaseProvider = commandLineModel.DatabaseProvider
        };
    }

    internal static DbProvider ValidateDatabaseProvider(string databaseProviderString, ILogger logger)
    {
        if (string.IsNullOrEmpty(databaseProviderString))
        {
            logger.LogMessage("NoDbProviderFound", LogMessageLevel.Information);
            return DbProvider.SqlServer;
        }
        else if (Enum.TryParse(databaseProviderString, ignoreCase: true, out DbProvider dbProvider))
        {
            return dbProvider;
        }
        else
        {
            string dbList = $"'{string.Join("', ", EfConstants.AllDbProviders.ToArray(), 0, EfConstants.AllDbProviders.Count - 1)} and '{EfConstants.AllDbProviders.LastOrDefault()}'";
            throw new InvalidOperationException($"Invalid database provider '{databaseProviderString}'.\nSupported database providers include : {dbList}");
        }
    }
}
