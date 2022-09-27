using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using ForeignExchange.Api.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ForeignExchange.Api.Tests.Integration;

public class ForexApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly TestcontainerDatabase _dbContainer =
        new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration
            {
                Database = "mydb",
                Username = "workshop",
                Password = "changeme"
            }).Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IDbConnectionFactory));
            services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgsqlConnectionFactory(_dbContainer.ConnectionString));
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
