namespace MinApiLib.DependencyInjection.Tests;

public class Default
{
    [Fact]
    public void IsRegistered()
    {
        var services = new ServiceCollection();
        services.AddAssembly(typeof(DefaultService).Assembly);

        var actual = services.Any(x => x.ServiceType == typeof(DefaultService));
        Assert.True(actual);
    }

    [Fact]
    public void WithTheSameType()
    {
        var services = new ServiceCollection();
        services.AddAssembly(typeof(DefaultService).Assembly);

        var actual = services.Any(x => x.ServiceType == typeof(DefaultService) && x.ImplementationType == typeof(DefaultService));
        Assert.True(actual);
    }

    [Fact]
    public void AndTransient()
    {
        var services = new ServiceCollection();
        services.AddAssembly(typeof(DefaultService).Assembly);

        var actual = services.Any(x => x.ServiceType == typeof(DefaultService) && x.ImplementationType == typeof(DefaultService) && x.Lifetime == ServiceLifetime.Transient);
        Assert.True(actual);
    }

    [RegisterInIServiceCollection]
    public class DefaultService
    {
    }
}