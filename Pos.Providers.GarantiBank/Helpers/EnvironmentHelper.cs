namespace Pos.Providers.GarantiBank.Helpers;

public static class EnvironmentHelper
{
    public static string? Get(string key) => Environment.GetEnvironmentVariable(key);

    public static bool IsDeveloper() =>
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true;
}

