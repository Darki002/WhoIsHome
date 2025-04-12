using System.Threading.RateLimiting;
using WhoIsHome.Host;
using WhoIsHome.Host.Authentication;
using WhoIsHome.Host.BackgroundTasks;
using WhoIsHome.Host.SetUp;
using WhoIsHome.Shared.BackgroundTasks;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddLoggers();

builder.Services
    .AddCorsPolicy()
    .AddApplicationServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

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
        
app.UseWihExceptionHandler();
app.UseMiddleware<ApiKeyMiddleware>();

app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.ConfigureDatabase();
app.Run();
