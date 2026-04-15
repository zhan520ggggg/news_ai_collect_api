namespace Domain.Entities;

/// <summary>
/// 用户-角色关联表
/// </summary>
public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    /// <summary>
    /// 关联用户
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// 关联角色
    /// </summary>
    public Role Role { get; set; } = null!;
}
