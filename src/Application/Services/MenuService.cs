using AutoMapper;
using Domain.Constants;
using Domain.Interfaces;
using Application.DTOs;
using Application.Interfaces;

namespace Application.Services;

/// <summary>
/// 菜单服务实现
/// </summary>
public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;
    private readonly IMapper _mapper;

    public MenuService(IMenuRepository menuRepository, IMapper mapper)
    {
        _menuRepository = menuRepository;
        _mapper = mapper;
    }

    public async Task<MenuResponseDto> CreateMenuAsync(CreateMenuDto dto, CancellationToken ct = default)
    {
        if (dto.ParentId.HasValue)
        {
            var parent = await _menuRepository.GetByIdAsync(dto.ParentId.Value, ct)
                ?? throw new Common.Models.BusinessException(404, "父菜单不存在");
        }

        var menu = _mapper.Map<Domain.Entities.Menu>(dto);
        var created = await _menuRepository.AddAsync(menu, ct);
        return _mapper.Map<MenuResponseDto>(created);
    }

    public async Task<MenuResponseDto> GetMenuByIdAsync(Guid id, CancellationToken ct = default)
    {
        var menu = await _menuRepository.GetByIdAsync(id, ct)
            ?? throw new Common.Models.NotFoundException(nameof(Domain.Entities.Menu), id);
        return _mapper.Map<MenuResponseDto>(menu);
    }

    public async Task<List<MenuResponseDto>> GetAllMenusAsync(CancellationToken ct = default)
    {
        var menus = await _menuRepository.GetAllAsync(ct);
        return _mapper.Map<List<MenuResponseDto>>(menus);
    }

    public async Task<List<MenuTreeDto>> GetMenuTreeAsync(CancellationToken ct = default)
    {
        var menus = await _menuRepository.GetMenuTreeAsync(ct);
        return BuildMenuTree(menus, null);
    }

    public async Task<MenuResponseDto> UpdateMenuAsync(
        Guid id, UpdateMenuDto dto, CancellationToken ct = default)
    {
        var menu = await _menuRepository.GetByIdAsync(id, ct)
            ?? throw new Common.Models.NotFoundException(nameof(Domain.Entities.Menu), id);

        if (dto.ParentId.HasValue && dto.ParentId.Value != menu.ParentId)
        {
            var parent = await _menuRepository.GetByIdAsync(dto.ParentId.Value, ct)
                ?? throw new Common.Models.BusinessException(404, "父菜单不存在");
        }

        _mapper.Map(dto, menu);
        menu.UpdatedAt = DateTime.UtcNow;

        await _menuRepository.UpdateAsync(menu, ct);
        return _mapper.Map<MenuResponseDto>(menu);
    }

    public async Task DeleteMenuAsync(Guid id, CancellationToken ct = default)
    {
        var menu = await _menuRepository.GetByIdAsync(id, ct)
            ?? throw new Common.Models.NotFoundException(nameof(Domain.Entities.Menu), id);

        var children = await _menuRepository.GetAllAsync(ct);
        if (children.Any(m => m.ParentId == id))
            throw new Common.Models.BusinessException(400, "该菜单下存在子菜单，无法删除");

        await _menuRepository.DeleteAsync(menu, ct);
    }

    public async Task<List<MenuTreeDto>> GetUserMenusAsync(
        Guid userId, IUserRepository userRepository, CancellationToken ct = default)
    {
        // 获取用户角色名称
        var roleNames = await userRepository.GetUserRoleNamesAsync(userId, ct);
        if (!roleNames.Any())
            return new List<MenuTreeDto>();

        // 获取完整菜单树
        var allMenus = await _menuRepository.GetMenuTreeAsync(ct);

        // 如果是超级管理员，直接返回所有菜单
        if (roleNames.Contains(RoleNames.SuperAdmin))
            return BuildMenuTree(allMenus, null);

        // 获取用户角色关联的菜单 Id
        var menuIds = allMenus
            .Where(m => m.RoleMenus.Any(rm => roleNames.Contains(rm.Role.Name)))
            .Select(m => m.Id)
            .ToHashSet();

        // 过滤用户可见的菜单
        var visibleMenus = allMenus.Where(m => menuIds.Contains(m.Id) && m.Visible).ToList();

        return BuildMenuTree(visibleMenus, null);
    }

    private List<MenuTreeDto> BuildMenuTree(List<Domain.Entities.Menu> allMenus, Guid? parentId)
    {
        return allMenus
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.Sort)
            .Select(m => new MenuTreeDto
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                ParentId = m.ParentId,
                Icon = m.Icon,
                Route = m.Route,
                Type = m.Type,
                Sort = m.Sort,
                Children = BuildMenuTree(allMenus, m.Id)
            })
            .ToList();
    }
}
