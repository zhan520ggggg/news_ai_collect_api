using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// 菜单仓储实现
/// </summary>
public class MenuRepository : Repository<Menu>, IMenuRepository
{
    public MenuRepository(ApplicationDbContext context) : base(context) { }

    public async Task<List<Menu>> GetMenuTreeAsync(CancellationToken ct = default) =>
        await _dbSet
            .Include(m => m.RoleMenus)
                .ThenInclude(rm => rm.Role)
            .OrderBy(m => m.Sort)
            .ThenBy(m => m.Id)
            .ToListAsync(ct);
}
