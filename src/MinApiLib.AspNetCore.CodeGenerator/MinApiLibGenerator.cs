using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.VisualStudio.Web;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc;

namespace MinApiLib.AspNetCore.CodeGenerator;

[Alias("minapilib")]
public class MinApiLibCodeGenerator : ICodeGenerator
{
    private IServiceProvider ServiceProvider { get; set; }
    private IApplicationInfo AppInfo { get; set; }
    private ILogger Logger { get; set; }
    private IModelTypesLocator ModelTypesLocator { get; set; }
    private IFileSystem FileSystem { get; set; }
    private IProjectContext ProjectContext { get; set; }
    private IEntityFrameworkService EntityFrameworkService { get; set; }
    private ICodeGeneratorActionsService CodeGeneratorActionsService { get; set; }
    private Workspace Workspace { get; set; }
    private IConsoleLogger ConsoleLogger { get; set; }

    public MinApiLibCodeGenerator(
        IApplicationInfo applicationInfo,
        IServiceProvider serviceProvider,
        IModelTypesLocator modelTypesLocator,
        ILogger logger,
        IFileSystem fileSystem,
        ICodeGeneratorActionsService codeGeneratorActionsService,
        IProjectContext projectContext,
        IEntityFrameworkService entityframeworkService,
        Workspace workspace)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider)); ;
        Logger = logger ?? throw new ArgumentNullException(nameof(logger)); ;
        AppInfo = applicationInfo ?? throw new ArgumentNullException(nameof(applicationInfo)); ;
        ModelTypesLocator = modelTypesLocator ?? throw new ArgumentNullException(nameof(modelTypesLocator));
        FileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        CodeGeneratorActionsService = codeGeneratorActionsService ?? throw new ArgumentNullException(nameof(codeGeneratorActionsService));
        ProjectContext = projectContext ?? throw new ArgumentNullException(nameof(projectContext));
        Workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));
        EntityFrameworkService = entityframeworkService ?? throw new ArgumentNullException(nameof(entityframeworkService));
        ConsoleLogger = new ConsoleLogger(jsonOutput: false);
    }

    public async Task GenerateCode(MinApiLibCommandLineGeneratorModel model)
    {
        model.ValidateCommandline(Logger);

        var namespaceName = NameSpaceUtilities.GetSafeNameSpaceFromPath(model.RelativeFolderPath, AppInfo.ApplicationName);
        //get model and dbcontext
        var modelTypeAndContextModel = await ModelMetadataUtilities.GetModelEFMetadataMinApiLibAsync(
            model,
            EntityFrameworkService,
            ModelTypesLocator,
            Logger,
            areaName: string.Empty);

        Console.WriteLine("Custom generator");
    }
}
