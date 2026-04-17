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

        // 种子菜单（仅当菜单表为空时）
        if (!await ctx.Menus.AnyAsync())
        {
            var modules = new List<Menu>
            {
                // 4 个顶级模块 (type=0)
                new Menu { Code = MenuCodes.Collection, Name = "热点采集模块", Icon = "Collection", Route = "/collection", Type = 0, Sort = 10 },
                new Menu { Code = MenuCodes.Hotspot, Name = "热点管理模块", Icon = "Hotspot", Route = "/hotspot", Type = 0, Sort = 20 },
                new Menu { Code = MenuCodes.DataAnalysis, Name = "数据分析展示模块", Icon = "DataAnalysis", Route = "/data-analysis", Type = 0, Sort = 30 },
                new Menu { Code = MenuCodes.System, Name = "系统管理模块", Icon = "Settings", Route = "/system", Type = 0, Sort = 40 },
            };
            await ctx.Menus.AddRangeAsync(modules);
            await ctx.SaveChangesAsync();

            var parentId = modules.ToDictionary(m => m.Code, m => m.Id);
            var subMenus = new List<Menu>
            {
                // 热点采集模块子菜单 (type=1)
                new Menu { Code = MenuCodes.CollectionConfig, Name = "采集配置", ParentId = parentId[MenuCodes.Collection], Route = "/collection/config", Type = 1, Sort = 10 },
                new Menu { Code = MenuCodes.CollectionMonitor, Name = "采集执行与监控", ParentId = parentId[MenuCodes.Collection], Route = "/collection/monitor", Type = 1, Sort = 20 },
                new Menu { Code = MenuCodes.CollectionData, Name = "采集数据处理", ParentId = parentId[MenuCodes.Collection], Route = "/collection/data", Type = 1, Sort = 30 },

                // 热点管理模块子菜单 (type=1)
                new Menu { Code = MenuCodes.HotspotList, Name = "热点列表管理", ParentId = parentId[MenuCodes.Hotspot], Route = "/hotspot/list", Type = 1, Sort = 10 },
                new Menu { Code = MenuCodes.HotspotEdit, Name = "热点编辑", ParentId = parentId[MenuCodes.Hotspot], Route = "/hotspot/edit", Type = 1, Sort = 20 },
                new Menu { Code = MenuCodes.HotspotCategoryTag, Name = "分类与标签管理", ParentId = parentId[MenuCodes.Hotspot], Route = "/hotspot/category-tag", Type = 1, Sort = 30 },
                new Menu { Code = MenuCodes.HotspotReview, Name = "热点审核流程", ParentId = parentId[MenuCodes.Hotspot], Route = "/hotspot/review", Type = 1, Sort = 40 },
                new Menu { Code = MenuCodes.HotspotPublish, Name = "热点发布与下架", ParentId = parentId[MenuCodes.Hotspot], Route = "/hotspot/publish", Type = 1, Sort = 50 },
                new Menu { Code = MenuCodes.HotspotArchive, Name = "热点归档", ParentId = parentId[MenuCodes.Hotspot], Route = "/hotspot/archive", Type = 1, Sort = 60 },

                // 数据分析展示模块子菜单 (type=1)
                new Menu { Code = MenuCodes.DataDashboard, Name = "核心数据仪表盘", ParentId = parentId[MenuCodes.DataAnalysis], Route = "/data-analysis/dashboard", Type = 1, Sort = 10 },
                new Menu { Code = MenuCodes.DataTrend, Name = "热点热度分析", ParentId = parentId[MenuCodes.DataAnalysis], Route = "/data-analysis/trend", Type = 1, Sort = 20 },
                new Menu { Code = MenuCodes.DataCollection, Name = "采集数据分析", ParentId = parentId[MenuCodes.DataAnalysis], Route = "/data-analysis/collection", Type = 1, Sort = 30 },
                new Menu { Code = MenuCodes.DataOperations, Name = "运营数据分析", ParentId = parentId[MenuCodes.DataAnalysis], Route = "/data-analysis/operations", Type = 1, Sort = 40 },

                // 系统管理模块子菜单 (type=1)
                new Menu { Code = MenuCodes.SystemRole, Name = "菜单管理", ParentId = parentId[MenuCodes.System], Route = "/system/menus", Type = 1, Sort = 10 },
                new Menu { Code = MenuCodes.SystemRole, Name = "角色与权限管理", ParentId = parentId[MenuCodes.System], Route = "/system/role", Type = 1, Sort = 10 },
                new Menu { Code = MenuCodes.SystemConfig, Name = "系统参数配置", ParentId = parentId[MenuCodes.System], Route = "/system/config", Type = 1, Sort = 20 },
                new Menu { Code = MenuCodes.SystemBackup, Name = "数据备份与恢复", ParentId = parentId[MenuCodes.System], Route = "/system/backup", Type = 1, Sort = 30 },
                new Menu { Code = MenuCodes.SystemNotification, Name = "息通知管理", ParentId = parentId[MenuCodes.System], Route = "/system/notification", Type = 1, Sort = 40 },
            };
            await ctx.Menus.AddRangeAsync(subMenus);
            await ctx.SaveChangesAsync();

            // 将所有菜单关联到 SuperAdmin 角色
            var superAdminRole = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.SuperAdmin);
            if (superAdminRole != null)
            {
                var allMenus = await ctx.Menus.ToListAsync();
                var roleMenus = allMenus.Select(m => new RoleMenu
                {
                    RoleId = superAdminRole.Id,
                    MenuId = m.Id
                }).ToList();
                await ctx.RoleMenus.AddRangeAsync(roleMenus);
                await ctx.SaveChangesAsync();
            }
        }
    }
}
