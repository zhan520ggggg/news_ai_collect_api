namespace Domain.Entities;

/// <summary>
/// 菜单实体（树形结构）
/// </summary>
public class Menu : BaseEntity
{
    /// <summary>
    /// 菜单编码（唯一标识，如 collection、hotspot_list）
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 菜单名称（显示文本）
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 父菜单 Id（null 表示顶级模块）
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 前端路由路径
    /// </summary>
    public string? Route { get; set; }

    /// <summary>
    /// 菜单类型：0=模块, 1=菜单, 2=按钮
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 排序值
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// 父菜单导航
    /// </summary>
    public Menu? Parent { get; set; }

    /// <summary>
    /// 子菜单集合
    /// </summary>
    public ICollection<Menu> Children { get; set; } = new List<Menu>();

    /// <summary>
    /// 角色-菜单关联
    /// </summary>
    public ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
