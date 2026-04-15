using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using Common.Models;

namespace WebApi.Controllers;

/// <summary>
/// 角色管理接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// 获取所有角色（含菜单 Id 列表）
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<RoleMenuResponseDto>>), 200)]
    public async Task<IActionResult> GetAllRoles(CancellationToken ct)
    {
        var result = await _roleService.GetAllRolesWithMenusAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// 获取指定角色的菜单
    /// </summary>
    [HttpGet("{id:guid}/menus")]
    [ProducesResponseType(typeof(ApiResponse<RoleMenuResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetRoleMenus(Guid id, CancellationToken ct)
    {
        var result = await _roleService.GetRoleMenusAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// 为角色分配菜单
    /// </summary>
    [HttpPut("{id:guid}/menus")]
    [ProducesResponseType(typeof(ApiResponse<RoleMenuResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AssignRoleMenus(
        Guid id, [FromBody] AssignMenusDto dto, CancellationToken ct)
    {
        var result = await _roleService.AssignMenusToRoleAsync(id, dto, ct);
        return Ok(result);
    }
}
