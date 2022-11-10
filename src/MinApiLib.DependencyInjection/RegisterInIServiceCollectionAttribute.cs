namespace MinApiLib.DependencyInjection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class RegisterInIServiceCollectionAttribute : Attribute
{
    public RegisterInIServiceCollectionAttribute()
    {
    }

    public RegisterInIServiceCollectionAttribute(Type type)
    {
        Type = type;
    }

    public RegisterInIServiceCollectionAttribute(Type type, ServiceLifetime serviceLifetime)
    {
        Type = type;
        ServiceLifetime = serviceLifetime;
    }

    public Type Type { get; protected set; }

    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Transient;
}