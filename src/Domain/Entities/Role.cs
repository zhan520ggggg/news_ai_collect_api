using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// 角色实体
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// 角色名称（英文标识，如 SuperAdmin）
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色显示名称（如 超级管理员）
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    [MaxLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// 该角色关联的用户
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// 该角色关联的菜单
    /// </summary>
    public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
