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

            object? testClient = await manager.FindByClientIdAsync("postman", cancellationToken);

            if (testClient == null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "postman",
                    ClientSecret = "testclient-secret",
                    DisplayName = "Postman",
                    RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,

                        OpenIddictConstants.Permissions.Endpoints.Token,

                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

                        OpenIddictConstants.Permissions.Prefixes.Scope + "api",

                        OpenIddictConstants.Permissions.ResponseTypes.Code
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
