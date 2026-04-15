using Application.DTOs;
using Common.Models;

namespace Application.Interfaces;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 创建用户
    /// </summary>
    Task<UserResponseDto> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default);

    /// <summary>
    /// 根据 ID 获取用户
    /// </summary>
    Task<UserResponseDto> GetUserByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// 获取所有用户
    /// </summary>
    Task<IReadOnlyList<UserResponseDto>> GetAllUsersAsync(CancellationToken ct = default);

    /// <summary>
    /// 分页获取用户
    /// </summary>
    Task<PagedResponse<UserResponseDto>> GetUsersPagedAsync(
        PagedRequest request, CancellationToken ct = default);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task<UserResponseDto> UpdateUserAsync(
        Guid id, UpdateUserDto dto, CancellationToken ct = default);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task DeleteUserAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// 用户登录
    /// </summary>
    Task<LoginResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default);

    /// <summary>
    /// 用户注册
    /// </summary>
    Task<UserResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default);
}
