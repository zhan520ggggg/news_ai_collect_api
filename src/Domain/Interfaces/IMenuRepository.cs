using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// 菜单仓储接口
/// </summary>
public interface IMenuRepository : IRepository<Menu>
{
    /// <summary>
    /// 获取完整菜单树（按 Sort 排序）
    /// </summary>
    Task<List<Menu>> GetMenuTreeAsync(CancellationToken ct = default);
}
