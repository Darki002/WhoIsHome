using WhoIsHome;

var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.Load();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
});
builder.Services.AddControllers();

// Add Services
builder.Services.AddWhoIsHomeServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapControllers();
app.UseAuthorization();
app.UseHttpsRedirection();

app.Run();
