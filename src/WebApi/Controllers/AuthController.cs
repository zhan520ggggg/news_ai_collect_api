using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using Common.Models;

namespace WebApi.Controllers;

/// <summary>
/// 认证接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="dto">登录信息，包含用户名和密码</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>登录成功返回 JWT Token 和用户信息</returns>
    /// <response code="200">登录成功</response>
    /// <response code="401">用户名或密码错误</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _userService.LoginAsync(dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="dto">注册信息，包含用户名、邮箱、密码、显示名称和角色列表</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>注册成功返回用户信息</returns>
    /// <response code="200">注册成功</response>
    /// <response code="409">用户名或邮箱已存在</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto dto, CancellationToken ct)
    {
        var result = await _userService.RegisterAsync(dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 获取当前登录用户的信息
    /// </summary>
    /// <param name="ct">取消令牌</param>
    /// <returns>当前用户信息</returns>
    /// <response code="200">成功获取用户信息</response>
    /// <response code="401">未登录或 Token 无效</response>
    /// <response code="404">用户不存在</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value
            ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            throw new BusinessException(401, "无法识别当前用户");

        var result = await _userService.GetUserByIdAsync(id, ct);
        return Ok(result);
    }
}
