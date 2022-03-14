WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
//Add services to the container, configure MVC as we used an empty web template
builder.Services.AddControllersWithViews();

WebApplication? app = builder.Build();

//app.MapGet("/", () => "Hello World!");



//configure the http request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

app.Run();
