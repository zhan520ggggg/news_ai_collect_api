using Application.DTOs;

namespace Application.Interfaces;

/// <summary>
/// 菜单服务接口
/// </summary>
public interface IMenuService
{
    Task<MenuResponseDto> CreateMenuAsync(CreateMenuDto dto, CancellationToken ct = default);
    Task<MenuResponseDto> GetMenuByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<MenuResponseDto>> GetAllMenusAsync(CancellationToken ct = default);
    Task<List<MenuTreeDto>> GetMenuTreeAsync(CancellationToken ct = default);
    Task<MenuResponseDto> UpdateMenuAsync(Guid id, UpdateMenuDto dto, CancellationToken ct = default);
    Task DeleteMenuAsync(Guid id, CancellationToken ct = default);
    Task<List<MenuTreeDto>> GetUserMenusAsync(
        Guid userId, Domain.Interfaces.IUserRepository userRepository, CancellationToken ct = default);
}
