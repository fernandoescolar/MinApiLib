namespace MinApiLib.DependencyInjection.Tests;

public class InterfacedScoped
{
    [Fact]
    public void IsRegisteredAsInterface()
    {
        var services = new ServiceCollection();
        services.AddAssembly(typeof(InterfacedScopedService).Assembly);

        var actual = services.Any(x => x.ServiceType == typeof(IInterfacedScopedService));
        Assert.True(actual);
    }

    [Fact]
    public void WithRightImplementation()
    {
        var services = new ServiceCollection();
        services.AddAssembly(typeof(InterfacedScopedService).Assembly);

        var actual = services.Any(x => x.ServiceType == typeof(IInterfacedScopedService) && x.ImplementationType == typeof(InterfacedScopedService));
        Assert.True(actual);
    }

    [Fact]
    public void AndScoped()
    {
        var services = new ServiceCollection();
        services.AddAssembly(typeof(InterfacedScopedService).Assembly);

        var actual = services.Any(x => x.ServiceType == typeof(IInterfacedScopedService) && x.ImplementationType == typeof(InterfacedScopedService) && x.Lifetime == ServiceLifetime.Scoped);
        Assert.True(actual);
    }

    public interface IInterfacedScopedService
    {
    }

    [RegisterInIServiceCollection(typeof(IInterfacedScopedService), ServiceLifetime.Scoped)]
    public class InterfacedScopedService : IInterfacedScopedService
    {
    }
}