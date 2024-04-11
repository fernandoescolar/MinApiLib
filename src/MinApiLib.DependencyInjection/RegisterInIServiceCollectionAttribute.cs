namespace MinApiLib.DependencyInjection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class RegisterInIServiceCollectionAttribute : Attribute
{
    private readonly Type[] _types;
    public RegisterInIServiceCollectionAttribute(params Type[] types)
    {
        _types = types;
    }

    public RegisterInIServiceCollectionAttribute(Type type)
    {
        _types = new []{ type };
    }

    public RegisterInIServiceCollectionAttribute(Type type, ServiceLifetime serviceLifetime)
        : this(type)
    {
        ServiceLifetime = serviceLifetime;
    }

    internal IEnumerable<Type> Types => _types;

    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
}
