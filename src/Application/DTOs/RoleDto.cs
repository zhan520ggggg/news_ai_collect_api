namespace Application.DTOs;

/// <summary>
/// 创建角色请求
/// </summary>
public class CreateRoleDto
{
    /// <summary>
    /// 角色名称（英文标识，如 SuperAdmin）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色显示名称（如 超级管理员）
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// 更新角色请求（部分更新）
/// </summary>
public class UpdateRoleDto
{
    /// <summary>
    /// 角色名称（英文标识，如 SuperAdmin）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 角色显示名称（如 超级管理员）
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// 角色响应
/// </summary>
public class RoleResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
