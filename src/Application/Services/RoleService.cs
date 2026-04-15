using Domain.Interfaces;
using Domain.Entities;
using Application.Interfaces;
using Application.DTOs;
using Common.Models;

namespace Application.Services;

/// <summary>
/// 角色服务实现
/// </summary>
public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roleRepository;
    private readonly IRepository<RoleMenu> _roleMenuRepository;
    private readonly IMenuRepository _menuRepository;

    public RoleService(
        IRepository<Role> roleRepository,
        IRepository<RoleMenu> roleMenuRepository,
        IMenuRepository menuRepository)
    {
        _roleRepository = roleRepository;
        _roleMenuRepository = roleMenuRepository;
        _menuRepository = menuRepository;
    }

    public async Task<List<RoleMenuResponseDto>> GetAllRolesWithMenusAsync(CancellationToken ct = default)
    {
        var roles = await _roleRepository.GetAllAsync(ct);
        var allRoleMenus = await _roleMenuRepository.GetAllAsync(ct);

        return roles.Select(r => new RoleMenuResponseDto
        {
            RoleId = r.Id,
            RoleName = r.Name,
            RoleDisplayName = r.DisplayName,
            MenuIds = allRoleMenus
                .Where(rm => rm.RoleId == r.Id)
                .Select(rm => rm.MenuId)
                .ToList()
        }).ToList();
    }

    public async Task<RoleMenuResponseDto> GetRoleMenusAsync(Guid roleId, CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, ct)
            ?? throw new NotFoundException(nameof(Role), roleId);

        var roleMenus = await _roleMenuRepository.FindAsync(rm => rm.RoleId == roleId, ct);
        var menuIds = roleMenus.Select(rm => rm.MenuId).ToList();

        return new RoleMenuResponseDto
        {
            RoleId = role.Id,
            RoleName = role.Name,
            RoleDisplayName = role.DisplayName,
            MenuIds = menuIds
        };
    }

    public async Task<RoleMenuResponseDto> AssignMenusToRoleAsync(
        Guid roleId, AssignMenusDto dto, CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdAsync(roleId, ct)
            ?? throw new NotFoundException(nameof(Role), roleId);

        // 验证菜单 Id 是否都存在
        if (dto.MenuIds.Any())
        {
            var allMenus = await _menuRepository.GetAllAsync(ct);
            var foundIds = allMenus.Where(m => dto.MenuIds.Contains(m.Id)).Select(m => m.Id).ToHashSet();
            var missingIds = dto.MenuIds.Where(id => !foundIds.Contains(id)).ToList();
            if (missingIds.Any())
                throw new BusinessException(404, $"以下菜单 Id 不存在: {string.Join(", ", missingIds)}");
        }

        // 删除旧关联
        var existing = await _roleMenuRepository.FindAsync(rm => rm.RoleId == roleId, ct);
        foreach (var rm in existing)
            await _roleMenuRepository.DeleteAsync(rm, ct);

        // 添加新关联
        foreach (var menuId in dto.MenuIds)
        {
            var roleMenu = new RoleMenu
            {
                RoleId = roleId,
                MenuId = menuId
            };
            await _roleMenuRepository.AddAsync(roleMenu, ct);
        }

        return new RoleMenuResponseDto
        {
            RoleId = role.Id,
            RoleName = role.Name,
            RoleDisplayName = role.DisplayName,
            MenuIds = dto.MenuIds
        };
    }
}
