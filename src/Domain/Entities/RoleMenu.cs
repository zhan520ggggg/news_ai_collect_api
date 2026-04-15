namespace Domain.Entities;

/// <summary>
/// 角色-菜单关联表
/// </summary>
public class RoleMenu : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid MenuId { get; set; }

    /// <summary>
    /// 关联角色
    /// </summary>
    public Role Role { get; set; } = null!;

    /// <summary>
    /// 关联菜单
    /// </summary>
    public Menu Menu { get; set; } = null!;
}
