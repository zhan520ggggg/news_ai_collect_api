using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

/// <summary>
/// 登录请求
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 注册请求
/// </summary>
public class RegisterDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码长度不能少于 6 位")]
    [MaxLength(50)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? DisplayName { get; set; }
}

/// <summary>
/// 登录响应
/// </summary>
public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserResponseDto User { get; set; } = null!;
    public List<MenuTreeDto> Menus { get; set; } = new();

    /// <summary>
    /// 用户角色列表
    /// </summary>
    public List<string> Roles { get; set; } = new();
}

/// <summary>
/// 角色信息 DTO
/// </summary>
public class RoleDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
