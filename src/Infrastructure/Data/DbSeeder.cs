using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

/// <summary>
/// 数据库种子数据
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// 初始化角色和默认超级管理员账号
    /// </summary>
    public static async Task SeedAsync(ApplicationDbContext ctx)
    {
        // 确保数据库已创建
        await ctx.Database.EnsureCreatedAsync();

        // 种子角色
        if (!await ctx.Roles.AnyAsync())
        {
            var roles = new[]
            {
                new Role { Name = RoleNames.SuperAdmin, DisplayName = "超级管理员", Description = "拥有系统所有权限" },
                new Role { Name = RoleNames.ContentReviewer, DisplayName = "内容审核员", Description = "负责内容审核与管理" },
                new Role { Name = RoleNames.CollectionAdmin, DisplayName = "采集管理员", Description = "负责数据采集与管理" },
                new Role { Name = RoleNames.DataAnalyst, DisplayName = "数据分析员", Description = "负责数据分析与报表" },
                new Role { Name = RoleNames.OperationsAdmin, DisplayName = "运营管理员", Description = "负责系统运营与配置" },
            };

            await ctx.Roles.AddRangeAsync(roles);
            await ctx.SaveChangesAsync();
        }

        // 种子默认超级管理员 (admin / Admin@123)
        if (!await ctx.Users.AnyAsync())
        {
            var superAdmin = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.SuperAdmin);
            if (superAdmin != null)
            {
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@system.local",
                    DisplayName = "系统管理员",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    IsActive = true
                };

                await ctx.Users.AddAsync(adminUser);
                await ctx.SaveChangesAsync();

                // 关联角色
                ctx.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = superAdmin.Id
                });
                await ctx.SaveChangesAsync();
            }
        }
    }
}
