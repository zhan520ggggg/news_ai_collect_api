namespace Application.DTOs;

/// <summary>
/// 菜单响应 DTO
/// </summary>
public class MenuResponseDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int Type { get; set; }
    public int Sort { get; set; }
    public bool Visible { get; set; }
}

/// <summary>
/// 菜单树响应 DTO（嵌套子菜单）
/// </summary>
public class MenuTreeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int Type { get; set; }
    public int Sort { get; set; }
    public List<MenuTreeDto> Children { get; set; } = new();
}

/// <summary>
/// 创建菜单请求
/// </summary>
public class CreateMenuDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int Type { get; set; }
    public int Sort { get; set; }
    public bool Visible { get; set; } = true;
}

/// <summary>
/// 更新菜单请求
/// </summary>
public class UpdateMenuDto
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public Guid? ParentId { get; set; }
    public string? Icon { get; set; }
    public string? Route { get; set; }
    public int? Type { get; set; }
    public int? Sort { get; set; }
    public bool? Visible { get; set; }
}

/// <summary>
/// 角色菜单响应
/// </summary>
public class RoleMenuResponseDto
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string RoleDisplayName { get; set; } = string.Empty;
    public List<Guid> MenuIds { get; set; } = new();
}

/// <summary>
/// 为角色分配菜单请求
/// </summary>
public class AssignMenusDto
{
    public List<Guid> MenuIds { get; set; } = new();
}
