using WhoIsHome.Host.Authentication;
using WhoIsHome.Host.SetUp;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddCorsPolicy()
    .AddApplicationServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

var app = builder.Build();
app.UseCorsPolicy();
app.ConfigureApplication();
app.ConfigureDatabase();
app.Run();
