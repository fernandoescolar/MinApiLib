namespace MinApiLib.HashedIds;

public static class HashedIdExtensions
{
    public static IServiceCollection AddHashedIds(this IServiceCollection services, Action<HashedIdOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new HashedIdOptions();
        configure?.Invoke(options);

        ArgumentNullException.ThrowIfNull(options.Passphrase, nameof(options.Passphrase));

        Hasher.Instance = new Hashids(options.Passphrase);
        services.AddSingleton<IHashids>(sp => Hasher.Instance);

        return services;
    }
}