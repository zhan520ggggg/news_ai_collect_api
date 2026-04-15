using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extensions;

/// <summary>
/// 应用层依赖注入扩展
/// 通过约定自动扫描注册所有服务实现
/// </summary>
public static class ApplicationServiceExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        //获取当前项目的程序集
        var assembly = typeof(ApplicationServiceExtension).Assembly;

        // 注册 AutoMapper（自动扫描所有 Profile 类）
        services.AddAutoMapper(assembly);

        // 注册 FluentValidation 验证器
        services.AddValidatorsFromAssembly(assembly);

        // 约定：Application.Services 命名空间下的具体类
        // 自动查找其实现的接口并注册为 Scoped
        var serviceTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, Namespace: not null }
                     && t.Namespace.StartsWith("Application.Services")
                     && !t.Name.Contains('<'));

        foreach (var implType in serviceTypes)
        {
            var interfaces = implType.GetInterfaces()
                .Where(i => i.Name.StartsWith('I')
                         && i.Namespace?.StartsWith("Application.Interfaces") == true)
                .ToList();

            if (interfaces.Count == 0) continue;

            // 单个接口 → 直接注册
            if (interfaces.Count == 1)
            {
                services.AddScoped(interfaces[0], implType);
            }
            // 多个接口 → 全部注册
            else
            {
                foreach (var iface in interfaces)
                    services.AddScoped(iface, implType);
            }
        }

        return services;
    }
}
