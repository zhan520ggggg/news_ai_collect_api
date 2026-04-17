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
    /// 创建新用户
    /// </summary>
    /// <param name="dto">用户信息，包含用户名、邮箱、密码和角色列表</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>创建的用户信息</returns>
    /// <response code="200">创建成功</response>
    /// <response code="409">用户名或邮箱已存在</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserDto dto, CancellationToken ct)
    {
        var result = await _userService.CreateUserAsync(dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 根据 ID 获取用户信息
    /// </summary>
    /// <param name="id">用户 ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>用户信息</returns>
    /// <response code="200">获取成功</response>
    /// <response code="404">用户不存在</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken ct)
    {
        var result = await _userService.GetUserByIdAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// 获取所有用户列表
    /// </summary>
    /// <param name="ct">取消令牌</param>
    /// <returns>所有用户列表</returns>
    /// <response code="200">获取成功</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UserResponseDto>>), 200)]
    public async Task<IActionResult> GetAllUsers(CancellationToken ct)
    {
        var result = await _userService.GetAllUsersAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// 分页获取用户列表
    /// </summary>
    /// <param name="request">分页参数，包含页码和每页数量</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>分页用户数据</returns>
    /// <response code="200">获取成功</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<UserResponseDto>>), 200)]
    public async Task<IActionResult> GetUsersPaged(
        [FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await _userService.GetUsersPagedAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// 更新用户信息（部分更新）
    /// </summary>
    /// <param name="id">用户 ID</param>
    /// <param name="dto">用户更新信息，可包含用户名、邮箱、显示名称和角色列表</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>更新后的用户信息</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">用户不存在</response>
    /// <response code="409">用户名或邮箱已存在</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> UpdateUser(
        Guid id, [FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        var result = await _userService.UpdateUserAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 删除指定用户（软删除）
    /// </summary>
    /// <param name="id">用户 ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>删除成功消息</returns>
    /// <response code="200">删除成功</response>
    /// <response code="404">用户不存在</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        await _userService.DeleteUserAsync(id, ct);
        return Ok(new { message = "用户删除成功" });
    }
}
