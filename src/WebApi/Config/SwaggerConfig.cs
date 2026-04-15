using Microsoft.OpenApi.Models;

namespace WebApi.Config;

/// <summary>
/// Swagger 配置扩展
/// </summary>
public static class SwaggerConfig
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Clean Architecture API",
                Description = "基于 .NET 8 的多层架构 API",
                Contact = new OpenApiContact
                {
                    Name = "开发团队",
                    Email = "dev@example.com"
                }
            });

            // 启用 XML 注释
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

            // 添加 Bearer Token 认证支持
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "请输入 JWT Token，格式: Bearer {token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    public static WebApplication UseCustomSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Clean Architecture API v1");
                options.RoutePrefix = string.Empty; // 将 Swagger UI 设置为默认首页
            });
        }

        return app;
    }
}
