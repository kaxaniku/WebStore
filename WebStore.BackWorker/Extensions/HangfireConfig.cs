using Hangfire;
using Microsoft.Data.SqlClient;

namespace WebStore.BackWorker.Extensions;

internal static class HangfireConfig
{
    public static void ConfigureHangfire(this HostApplicationBuilder builder, string connectionString)
    {
        EnsureDatabaseCreated(connectionString);

        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString, new Hangfire.SqlServer.SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.FromSeconds(15),
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));

        builder.Services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
        });
    }

    private static void EnsureDatabaseCreated(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString);
        string databaseName = builder.InitialCatalog;

        builder.InitialCatalog = "master";
        string masterConnectionString = builder.ConnectionString;

        for (int i = 0; i < 5; i++)
        {
            try
            {
                using var connection = new SqlConnection(masterConnectionString);
                connection.Open();

                string query = $"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{databaseName}') CREATE DATABASE [{databaseName}]";
                using var command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();

                return;
            }
            catch (SqlException)
            {
                if (i == 9) throw;

                Thread.Sleep(5000);
            }
        }
    }
}
