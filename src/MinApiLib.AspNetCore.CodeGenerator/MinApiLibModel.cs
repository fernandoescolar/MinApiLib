using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;

namespace MinApiLib.AspNetCore.CodeGenerator;

public class MinApiLibModel
{
    public MinApiLibModel(ModelType modelType, string dbContextFullTypeName)
    {
        if (modelType == null)
        {
            throw new ArgumentNullException(nameof(modelType));
        }

        if (dbContextFullTypeName == null)
        {
            throw new ArgumentNullException(nameof(dbContextFullTypeName));
        }

        ModelType = modelType;

        var classNameModel = new ClassNameModel(dbContextFullTypeName);

        ContextTypeName = classNameModel.ClassName;
        DbContextNamespace = classNameModel.NamespaceName;
    }

    public ModelType ModelType { get; private set; }

    public string ContextTypeName { get; private set; }

    public string DbContextNamespace { get; private set; }

    public string ModelTypeName => ModelType.Name;

    public HashSet<string> RequiredNamespaces
    {
        get
        {
            var requiredNamespaces = new SortedSet<string>(StringComparer.Ordinal);
            var modelTypeNamespace = ModelType.Namespace;
            if (!string.IsNullOrWhiteSpace(modelTypeNamespace))
            {
                requiredNamespaces.Add(modelTypeNamespace);
            }

            if (!string.IsNullOrWhiteSpace(DbContextNamespace))
            {
                requiredNamespaces.Add(DbContextNamespace);
            }

            return new HashSet<string>(requiredNamespaces);
        }
    }
}
