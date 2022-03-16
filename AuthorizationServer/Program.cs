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
}).AddServer(options =>
{
    options.AllowClientCredentialsFlow();
    options.SetTokenEndpointUris("/connect/token");

    //Encryption and signin of tokens
    options.AddEphemeralEncryptionKey()
    .AddEphemeralSigningKey();

    //register scopes
    options.RegisterScopes("api");

    //register the asp.net
    options.UseAspNetCore().EnableAuthorizationEndpointPassthrough();
});



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
app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

app.Run();
