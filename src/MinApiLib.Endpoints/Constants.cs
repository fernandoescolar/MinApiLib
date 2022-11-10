namespace MinApiLib.Endpoints;

internal static class Constants
{
    public const string Get = "GET";
    public const string Post = "POST";
    public const string Put = "PUT";
    public const string Delete = "DELETE";
    public const string Patch = "PATCH";

    public static readonly string[] MethodNames = new[] { "Handle", "HandleAsync" };
}
