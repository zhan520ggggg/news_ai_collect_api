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
            var existingMenuIds = allMenus.Select(m => m.Id).ToHashSet();
            var missingIds = dto.MenuIds.Where(id => !existingMenuIds.Contains(id)).ToList();
            if (missingIds.Any())
                throw new BusinessException(404, $"以下菜单 Id 不存在: {string.Join(", ", missingIds)}");
        }

        // 删除旧关联并添加新关联
        await using var transaction = await _roleMenuRepository.BeginTransactionAsync(ct);
        try
        {
            await _roleMenuRepository.DeleteManyAsync(rm => rm.RoleId == roleId, ct);

            if (dto.MenuIds.Any())
            {
                var roleMenus = dto.MenuIds
                    .Select(menuId => new RoleMenu
                    {
                        RoleId = roleId,
                        MenuId = menuId
                    })
                    .ToList();

                await _roleMenuRepository.AddRangeAsync(roleMenus, ct);
            }

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }

        return new RoleMenuResponseDto
        {
            RoleId = role.Id,
            RoleName = role.Name,
            RoleDisplayName = role.DisplayName,
            MenuIds = dto.MenuIds
        };
    }

    public async Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto, CancellationToken ct = default)
    {
        var role = new Role
        {
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Description = dto.Description
        };

        await _roleRepository.AddAsync(role, ct);
        return MapToDto(role);
    }

    public async Task<RoleResponseDto> UpdateRoleAsync(Guid id, UpdateRoleDto dto, CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Role), id);

        if (dto.Name != null)
            role.Name = dto.Name;

        if (dto.DisplayName != null)
            role.DisplayName = dto.DisplayName;

        if (dto.Description != null)
            role.Description = dto.Description;

        await _roleRepository.UpdateAsync(role, ct);
        return MapToDto(role);
    }

    public async Task DeleteRoleAsync(Guid id, CancellationToken ct = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Role), id);

        await _roleRepository.DeleteAsync(role, ct);
    }

    private static RoleResponseDto MapToDto(Role role)
    {
        return new RoleResponseDto
        {
            Id = role.Id,
            Name = role.Name,
            DisplayName = role.DisplayName,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }
}
