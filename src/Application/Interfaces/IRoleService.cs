using Application.DTOs;

namespace Application.Interfaces;

/// <summary>
/// 角色服务接口
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 获取所有角色（含菜单 Id 列表）
    /// </summary>
    Task<List<RoleMenuResponseDto>> GetAllRolesWithMenusAsync(CancellationToken ct = default);

    /// <summary>
    /// 获取指定角色的菜单
    /// </summary>
    Task<RoleMenuResponseDto> GetRoleMenusAsync(Guid roleId, CancellationToken ct = default);

    /// <summary>
    /// 为角色分配菜单
    /// </summary>
    Task<RoleMenuResponseDto> AssignMenusToRoleAsync(
        Guid roleId, AssignMenusDto dto, CancellationToken ct = default);
}
