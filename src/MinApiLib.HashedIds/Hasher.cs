namespace MinApiLib.HashedIds;

internal static class Hasher
{
    private static IHashids? _instance;

    public static IHashids Instance
    {
        get => _instance ?? throw new InvalidOperationException("HashedIds is not configured: call services.AddHashedIds() when the application starts.");
        set => _instance = value;
    }
}
