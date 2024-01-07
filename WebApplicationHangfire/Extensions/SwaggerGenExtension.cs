using Microsoft.OpenApi.Models;
using System.Reflection;

namespace WebApplicationHangfire.Extensions;

public static class SwaggerGenExtension
{
    public static void AddSwaggerGenConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Hangfire Dynamic",
                Description = "Dynamic Recurring Jobs",
            });

            var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
            options.IncludeXmlComments(xmlPath);
        });
    }

    public static void UseSwaggerConfig(this WebApplication app)
    {
        app.UseSwagger(options =>
        {
            options.SerializeAsV2 = true;
        });
        app.UseSwaggerUI(options =>
        {
            options.DefaultModelsExpandDepth(-1);
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        });
    }
}
