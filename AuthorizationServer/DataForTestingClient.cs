using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace AuthorizationServer
{
    public class DataForTestingClient : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public DataForTestingClient(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            IServiceScope? scope = _serviceProvider.CreateScope();

            DbContext? context = scope.ServiceProvider.GetRequiredService<DbContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);

            IOpenIddictApplicationManager? manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            object? testClient = await manager.FindByClientIdAsync("testclient", cancellationToken);

            if (testClient == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "testclient",
                    ClientSecret = "testclient-secret",
                    DisplayName = "TestClient",
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api"
                    }
                }, cancellationToken);
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
