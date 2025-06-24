namespace ProjectPRM392.Utilities;

public static class ConfigurationHelper
{
    public static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING_ELECTRONICSTOREDB")
            ?? configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string is not configured. Please set CONNECTION_STRING_ELECTRONICSTOREDB environment variable or update appsettings.json.");
        }

        return connectionString;
    }
}