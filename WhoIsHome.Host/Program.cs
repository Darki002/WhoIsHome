using WhoIsHome.Host.Authentication;
using WhoIsHome.Host.SetUp;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


// TODO: User - https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit?view=aspnetcore-8.0
// Use different policicy for normal endpoints and UserController(more sensitive)

var app = builder.Build();
app.ConfigureApplication();
app.Run();
