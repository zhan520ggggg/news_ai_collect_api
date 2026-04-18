namespace Application.DTOs;

/// <summary>
/// 创建用户请求
/// </summary>
public class CreateUserDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    /// <summary>
    /// 角色 ID 列表（如：["c16aecc9-f155-47da-92ff-2835d9d30c74"]）
    /// </summary>
    public List<string> RoleIds { get; set; } = new();
}

/// <summary>
/// 更新用户请求（部分更新）
/// </summary>
public class UpdateUserDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? DisplayName { get; set; }
    /// <summary>
    /// 角色 ID 列表（如：["c16aecc9-f155-47da-92ff-2835d9d30c74"]）
    /// </summary>
    public List<string>? RoleIds { get; set; }
}

/// <summary>
/// 用户响应
/// </summary>
public class UserResponseDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string>? Roles { get; set; } = new();
}
