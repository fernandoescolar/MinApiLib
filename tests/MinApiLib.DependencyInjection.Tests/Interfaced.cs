namespace MinApiLib.DependencyInjection.Tests;

public class Interfaced
{
    [Fact]
    public void IsRegisteredAsInterface()
    {
        var services = new ServiceCollection();
        services.AddAssembly(typeof(InterfacedService).Assembly);

        var actual = services.Any(x => x.ServiceType == typeof(IInterfacedService));
        Assert.True(actual);
    }

    [Fact]
    public void WithRightImplementation()
    {
        var services = new ServiceCollection();
        services.AddAssembly(typeof(InterfacedService).Assembly);

        var actual = services.Any(x => x.ServiceType == typeof(IInterfacedService) && x.ImplementationType == typeof(InterfacedService));
        Assert.True(actual);
    }

    [Fact]
    public void AndTransient()
    {
        var services = new ServiceCollection();
        services.AddAssembly(typeof(InterfacedService).Assembly);

        var actual = services.Any(x => x.ServiceType == typeof(IInterfacedService) && x.ImplementationType == typeof(InterfacedService) && x.Lifetime == ServiceLifetime.Transient);
        Assert.True(actual);
    }

    public interface IInterfacedService
    {
    }

    [RegisterInIServiceCollection(typeof(IInterfacedService))]
    public class InterfacedService : IInterfacedService
    {
    }
}