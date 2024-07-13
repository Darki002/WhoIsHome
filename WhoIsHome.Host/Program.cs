using WhoIsHome;
using WhoIsHome.Infrastructure;
using WhoIsHome.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Add Services
builder.Services
    .AddWebApi()
    .AddWhoIsHomeServices()
    .AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.UseAuthorization();
app.UseHttpsRedirection();

app.Run();
