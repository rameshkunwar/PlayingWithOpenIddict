using Microsoft.AspNetCore.Authentication.Cookies;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
//Add services to the container, configure MVC as we used an empty web template
builder.Services.AddControllersWithViews();

//enable authentication, cookie based
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/account/login";
    });

WebApplication? app = builder.Build();

//app.MapGet("/", () => "Hello World!");

//configure the http request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

app.Run();
