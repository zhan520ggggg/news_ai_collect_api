using System.Text;
using Application.Extensions;
using Common.Models;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using WebApi.Config;
using WebApi.Filters;
using WebApi.Middleware;
using Infrastructure.Data;
using Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

try
{
    Log.Information("Starting Clean Architecture API");

    // 配置 Serilog
    builder.Host.UseSerilog();

    // ==================== 服务注册 ====================

    // JWT 配置
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
        ?? throw new InvalidOperationException("JWT Settings not configured");

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero
            };
        });

    builder.Services.AddAuthorization();

    // 跨域配置
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy
                .WithOrigins(
                    builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
                    ?? Array.Empty<string>())
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // 控制器 + 统一响应过滤器
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ApiResponseFilter>();
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // 将模型验证错误统一为 ApiResponse 格式
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new ApiResponse(422, string.Join("; ", errors));
            return new Microsoft.AspNetCore.Mvc.ObjectResult(response)
            {
                StatusCode = 422
            };
        };
    });

    // Swagger 规范化配置
    builder.Services.AddCustomSwagger();

    // FluentValidation（Web 层集成：模型绑定自动触发验证）
    builder.Services.AddFluentValidationAutoValidation();

    // 应用层：AutoMapper + FluentValidation 扫描 + 服务自动注册
    builder.Services.AddApplication();

    // 基础设施层：数据库 + 仓储 + JWT Token 提供者
    builder.Services.AddInfrastructure(builder.Configuration);

    // 配置选项
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

    var app = builder.Build();

    // ==================== 中间件管道 ====================

    // 全局异常处理（必须放在最前面）
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // 开发环境 Swagger UI
    app.UseCustomSwagger();

    // 跨域中间件
    app.UseCors();

    // HTTPS 重定向
    app.UseHttpsRedirection();

    // 路由
    app.UseRouting();

    // 认证 + 鉴权（必须在 UseRouting 之后，MapControllers 之前）
    app.UseAuthentication();
    app.UseAuthorization();

    // 控制器端点
    app.MapControllers();

    // 健康检查端点
    app.MapGet("/health", () => Results.Ok(new ApiResponse(0, "服务运行正常")));

    // ==================== 数据库初始化 ====================

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await DbSeeder.SeedAsync(context);
    }

    Log.Information("Clean Architecture API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
