using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// 用户仓储接口
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// 根据用户名查找
    /// </summary>
    Task<User?> FindByUserNameAsync(string userName, CancellationToken ct = default);

    /// <summary>
    /// 根据邮箱查找
    /// </summary>
    Task<User?> FindByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// 检查用户名是否已存在
    /// </summary>
    Task<bool> IsUserNameExistsAsync(string userName, CancellationToken ct = default);

    /// <summary>
    /// 检查邮箱是否已存在
    /// </summary>
    Task<bool> IsEmailExistsAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// 根据用户名查找（含角色信息）
    /// </summary>
    Task<User?> FindByUserNameWithRolesAsync(string userName, CancellationToken ct = default);
}
