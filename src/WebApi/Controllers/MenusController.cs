using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using Common.Models;

namespace WebApi.Controllers;

/// <summary>
/// 菜单管理接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    /// <summary>
    /// 获取完整菜单树（层级结构）
    /// </summary>
    /// <param name="ct">取消令牌</param>
    /// <returns>菜单树结构</returns>
    /// <response code="200">获取成功</response>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(ApiResponse<List<MenuTreeDto>>), 200)]
    public async Task<IActionResult> GetMenuTree(CancellationToken ct)
    {
        var result = await _menuService.GetMenuTreeAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// 获取所有菜单（扁平列表）
    /// </summary>
    /// <param name="ct">取消令牌</param>
    /// <returns>所有菜单列表</returns>
    /// <response code="200">获取成功</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<MenuResponseDto>>), 200)]
    public async Task<IActionResult> GetAllMenus(CancellationToken ct)
    {
        var result = await _menuService.GetAllMenusAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// 根据 ID 获取菜单信息
    /// </summary>
    /// <param name="id">菜单 ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>菜单信息</returns>
    /// <response code="200">获取成功</response>
    /// <response code="404">菜单不存在</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMenuById(Guid id, CancellationToken ct)
    {
        var result = await _menuService.GetMenuByIdAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// 创建新菜单
    /// </summary>
    /// <param name="dto">菜单信息，包含名称、路由、类型等</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>创建的菜单信息</returns>
    /// <response code="200">创建成功</response>
    /// <response code="404">父菜单不存在</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateMenu(
        [FromBody] CreateMenuDto dto, CancellationToken ct)
    {
        var result = await _menuService.CreateMenuAsync(dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 更新菜单信息
    /// </summary>
    /// <param name="id">菜单 ID</param>
    /// <param name="dto">菜单更新信息</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>更新后的菜单信息</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">菜单或父菜单不存在</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateMenu(
        Guid id, [FromBody] UpdateMenuDto dto, CancellationToken ct)
    {
        var result = await _menuService.UpdateMenuAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 删除指定菜单（软删除）
    /// </summary>
    /// <param name="id">菜单 ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>删除成功消息</returns>
    /// <response code="200">删除成功</response>
    /// <response code="400">菜单下存在子菜单，无法删除</response>
    /// <response code="404">菜单不存在</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteMenu(Guid id, CancellationToken ct)
    {
        await _menuService.DeleteMenuAsync(id, ct);
        return Ok(new ApiResponse(0, "删除成功"));
    }
}
