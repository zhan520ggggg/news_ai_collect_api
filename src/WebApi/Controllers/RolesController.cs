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
    /// 获取所有角色及其关联的菜单 ID 列表
    /// </summary>
    /// <param name="ct">取消令牌</param>
    /// <returns>所有角色及其菜单 ID 列表</returns>
    /// <response code="200">获取成功</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<RoleMenuResponseDto>>), 200)]
    public async Task<IActionResult> GetAllRoles(CancellationToken ct)
    {
        var result = await _roleService.GetAllRolesWithMenusAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// 获取指定角色的菜单 ID 列表
    /// </summary>
    /// <param name="id">角色 ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>角色及其菜单 ID 列表</returns>
    /// <response code="200">获取成功</response>
    /// <response code="404">角色不存在</response>
    [HttpGet("{id:guid}/menus")]
    [ProducesResponseType(typeof(ApiResponse<RoleMenuResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetRoleMenus(Guid id, CancellationToken ct)
    {
        var result = await _roleService.GetRoleMenusAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// 为指定角色分配菜单权限
    /// </summary>
    /// <param name="id">角色 ID</param>
    /// <param name="dto">菜单 ID 列表</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>分配后的角色菜单信息</returns>
    /// <response code="200">分配成功</response>
    /// <response code="404">角色或菜单不存在</response>
    [HttpPut("{id:guid}/menus")]
    [ProducesResponseType(typeof(ApiResponse<RoleMenuResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AssignRoleMenus(
        Guid id, [FromBody] AssignMenusDto dto, CancellationToken ct)
    {
        var result = await _roleService.AssignMenusToRoleAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 创建新角色
    /// </summary>
    /// <param name="dto">角色信息，包含角色名称、显示名称和描述</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>创建的角色信息</returns>
    /// <response code="201">创建成功</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), 201)]
    public async Task<IActionResult> CreateRole(
        [FromBody] CreateRoleDto dto, CancellationToken ct)
    {
        var result = await _roleService.CreateRoleAsync(dto, ct);
        return StatusCode(201, result);
    }

    /// <summary>
    /// 更新角色信息（部分更新）
    /// </summary>
    /// <param name="id">角色 ID</param>
    /// <param name="dto">角色更新信息，可包含名称、显示名称和描述</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>更新后的角色信息</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">角色不存在</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateRole(
        Guid id, [FromBody] UpdateRoleDto dto, CancellationToken ct)
    {
        var result = await _roleService.UpdateRoleAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 删除指定角色（软删除）
    /// </summary>
    /// <param name="id">角色 ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>删除成功</returns>
    /// <response code="204">删除成功</response>
    /// <response code="404">角色不存在</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteRole(Guid id, CancellationToken ct)
    {
        await _roleService.DeleteRoleAsync(id, ct);
        return NoContent();
    }
}
