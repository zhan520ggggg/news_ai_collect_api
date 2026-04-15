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
    /// 获取完整菜单树
    /// </summary>
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
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<MenuResponseDto>>), 200)]
    public async Task<IActionResult> GetAllMenus(CancellationToken ct)
    {
        var result = await _menuService.GetAllMenusAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// 根据 ID 获取菜单
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetMenuById(Guid id, CancellationToken ct)
    {
        var result = await _menuService.GetMenuByIdAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// 创建菜单
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MenuResponseDto>), 200)]
    public async Task<IActionResult> CreateMenu(
        [FromBody] CreateMenuDto dto, CancellationToken ct)
    {
        var result = await _menuService.CreateMenuAsync(dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 更新菜单
    /// </summary>
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
    /// 删除菜单
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteMenu(Guid id, CancellationToken ct)
    {
        await _menuService.DeleteMenuAsync(id, ct);
        return Ok(new ApiResponse(0, "删除成功"));
    }
}
