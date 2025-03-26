using System.Threading.RateLimiting;
using WhoIsHome.Host.Authentication;
using WhoIsHome.Host.BackgroundTasks;
using WhoIsHome.Host.SetUp;
using WhoIsHome.Shared.BackgroundTasks;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddCorsPolicy()
    .AddApplicationServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "dd.MM.yyyy HH:mm:ss";
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 50,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});

var app = builder.Build();
app.UseCorsPolicy();

app.UseSwagger();
app.UseSwaggerUI();
        
app.UseExceptionHandler();
app.UseMiddleware<ApiKeyMiddleware>();

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.ConfigureDatabase();
app.Run();
