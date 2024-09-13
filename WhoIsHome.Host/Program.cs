using WhoIsHome.Host.SetUp;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();
app.ConfigureApplication();
app.Run();
