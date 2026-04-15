using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using Common.Models;

namespace WebApi.Controllers;

/// <summary>
/// 用户管理接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserDto dto, CancellationToken ct)
    {
        var result = await _userService.CreateUserAsync(dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 根据 ID 获取用户
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken ct)
    {
        var result = await _userService.GetUserByIdAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserResponseDto>>), 200)]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var result = await _userService.GetAllUsersAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// 分页获取用户
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserResponseDto>>), 200)]
    public async Task<IActionResult> GetUsersPaged(
        [FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await _userService.GetUsersPagedAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateUser(
        Guid id, [FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        var result = await _userService.UpdateUserAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        await _userService.DeleteUserAsync(id, ct);
        return Ok(new { message = "用户删除成功" });
    }
}
