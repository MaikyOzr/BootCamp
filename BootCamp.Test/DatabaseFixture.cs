using Npgsql;
using System.Data.Common;
using Testcontainers.PostgreSql;

namespace BootCamp.Test;

public class DatabaseFixture : IAsyncLifetime
{
    private PostgreSqlContainer _container;
    public PostgreSqlContainer Container => _container;

    public string ConnectionString;

    public DbConnection CreateConnection() 
    {
        var connectionString = _container.GetConnectionString();

        return connectionString is null ? throw new InvalidOperationException("Connection string is null") : new NpgsqlConnection(connectionString);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync().ConfigureAwait(false);
        await _container.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("db")
            .WithUsername("postgres")
            .WithPassword("pwd")
            .Build();

        await _container.StartAsync();
        ConnectionString = _container.GetConnectionString();
    }
}
