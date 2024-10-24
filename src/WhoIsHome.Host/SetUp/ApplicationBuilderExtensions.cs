﻿using WhoIsHome.Host.Authentication;

namespace WhoIsHome.Host.SetUp;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureApplication(this WebApplication app)
    {
        if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseExceptionHandler();
        app.UseMiddleware<ApiKeyMiddleware>();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseHttpsRedirection();
    }
}