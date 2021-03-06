using AuthorizationServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
//Add services to the container, configure MVC as we used an empty web template

builder.Services.AddControllersWithViews();

//enable authentication, cookie based
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/account/login";
    });

//setting up OpenIddict
builder.Services.AddDbContext<DbContext>(options =>
{
    options.UseInMemoryDatabase(nameof(DbContext));

    // Configure the context to use an in-memory store.
    options.UseInMemoryDatabase(nameof(DbContext));

    // Register the entity sets needed by OpenIddict.
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict().AddCore(options =>
{
    options.UseEntityFrameworkCore().UseDbContext<DbContext>();
})
    // Register the OpenIddict server components.
    .AddServer(options =>
{
    //enables the flow
    //RequireProofKeyForCodeExchange is called directly after that, this makes sure all clients are required to use PKCE (Proof Key for Code Exchange).
    options.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
    options.AllowClientCredentialsFlow();
    options.SetTokenEndpointUris("/connect/token");
    options.SetAuthorizationEndpointUris("/connect/authorize");
    options.SetUserinfoEndpointUris("/connect/userinfo");

    //Encryption and signin of tokens
    options.AddEphemeralEncryptionKey()
            .AddEphemeralSigningKey()
            .DisableAccessTokenEncryption();

    //register scopes
    options.RegisterScopes("api");

    //register the asp.net core host and configure the asp.net core-specific options
    options
    .UseAspNetCore()
    .EnableTokenEndpointPassthrough()
    .EnableAuthorizationEndpointPassthrough()
    .EnableUserinfoEndpointPassthrough();
});

//let's register test client data
builder.Services.AddHostedService<DataForTestingClient>();


WebApplication? app = builder.Build();

//app.MapGet("/", () => "Hello World!");

//configure the http request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

app.Run();
