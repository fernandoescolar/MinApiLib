namespace MinApiLib.DependencyInjection.Tests;

public class MultipleTypes
{
    private readonly ServiceCollection _services;

    public MultipleTypes()
    {
        _services = new ServiceCollection();
        _services.AddAssembly(typeof(MultiInterfacedService).Assembly);
    }

    [Fact]
    public void IsRegisteredAsSelf()
    {
        var actual = _services.Any(x => x.ServiceType == typeof(MultiInterfacedService));
        Assert.True(actual);
    }

    [Fact]
    public void InterfacesAreRegistered()
    {
        var actual = _services.Any(x => x.ServiceType == typeof(IInterfacedScopedService1));
        Assert.True(actual);

        actual = _services.Any(x => x.ServiceType == typeof(IInterfacedScopedService2));
        Assert.True(actual);
    }

    [Fact]
    public void SelfRegistrationIsSingleton()
    {
        var actual = _services.FirstOrDefault(x => x.ServiceType == typeof(MultiInterfacedService));
        Assert.NotNull(actual);
        Assert.Equal(ServiceLifetime.Singleton, actual.Lifetime);
    }

     [Fact]
    public void InterfacesRegistrationsAreTransient()
    {
        var actual = _services.FirstOrDefault(x => x.ServiceType == typeof(IInterfacedScopedService1));
        Assert.NotNull(actual);
        Assert.Equal(ServiceLifetime.Transient, actual.Lifetime);

        actual = _services.FirstOrDefault(x => x.ServiceType == typeof(IInterfacedScopedService2));
         Assert.NotNull(actual);
        Assert.Equal(ServiceLifetime.Transient, actual.Lifetime);
    }

    [Fact]
    public void UsesTheSameInstance()
    {
        var serviceProvider = _services.BuildServiceProvider();
        var instance1 = serviceProvider.GetRequiredService<IInterfacedScopedService1>();
        var instance2 = serviceProvider.GetRequiredService<IInterfacedScopedService2>();

        Assert.Same(instance1, instance2);
    }

    public interface IInterfacedScopedService1
    {
    }
    public interface IInterfacedScopedService2
    {
    }

    [RegisterInIServiceCollection(typeof(IInterfacedScopedService1), typeof(IInterfacedScopedService2), ServiceLifetime = ServiceLifetime.Singleton)]
    public class MultiInterfacedService : IInterfacedScopedService1, IInterfacedScopedService2
    {
    }
}