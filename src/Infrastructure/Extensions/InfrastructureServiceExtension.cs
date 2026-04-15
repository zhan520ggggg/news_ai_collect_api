using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Jwt;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

/// <summary>
/// 基础设施层依赖注入扩展
/// </summary>
public static class InfrastructureServiceExtension
{
    /// <summary>
    /// 注册数据库和仓储
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // 数据库配置（默认使用 SQLite）
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(
                configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=app.db"));

        // 注册仓储
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();

        // 注册 JWT Token 提供者
        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();

        return services;
    }
}
