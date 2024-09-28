using WhoIsHome.Host.Authentication;
using WhoIsHome.Host.SetUp;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Services.AddApplicationServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();
app.ConfigureApplication();
app.Run();
