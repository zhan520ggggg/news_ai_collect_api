using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using Common.Models;

namespace WebApi.Controllers;

/// <summary>
/// 产品管理接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// 创建新产品
    /// </summary>
    /// <param name="dto">产品信息，包含名称、描述、价格等</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>创建的产品信息</returns>
    /// <response code="200">创建成功</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var result = await _productService.CreateProductAsync(dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 根据 ID 获取产品信息
    /// </summary>
    /// <param name="id">产品 ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>产品信息</returns>
    /// <response code="200">获取成功</</response>
    /// <response code="404">产品不存在</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProduct(Guid id, CancellationToken ct)
    {
        var result = await _productService.GetProductByIdAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// 获取所有产品列表
    /// </summary>
    /// <param name="ct">取消令牌</param>
    /// <returns>所有产品列表</returns>
    /// <response code="200">获取成功</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProductResponseDto>>), 200)]
    public async Task<IActionResult> GetAllProducts(CancellationToken ct)
    {
        var result = await _productService.GetAllProductsAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// 分页获取产品列表
    /// </summary>
    /// <param name="request">分页参数，包含页码和每页数量</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>分页产品数据</returns>
    /// <response code="200">获取成功</response>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProductResponseDto>>), 200)]
    public async Task<IActionResult> GetProductsPaged(
        [FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await _productService.GetProductsPagedAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// 更新产品信息
    /// </summary>
    /// <param name="id">产品 ID</param>
    /// <param name="dto">产品更新信息</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>更新后的产品信息</returns>
    /// <response code="200">更新成功</response>
    /// <response code="404">产品不存在</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateProduct(
        Guid id, [FromBody] UpdateProductDto dto, CancellationToken ct)
    {
        var result = await _productService.UpdateProductAsync(id, dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 删除指定产品（软删除）
    /// </summary>
    /// <param name="id">产品 ID</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>删除成功消息</returns>
    /// <response code="200">删除成功</response>
    /// <response code="404">产品不存在</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        await _productService.DeleteProductAsync(id, ct);
        return Ok(new { message = "产品删除成功" });
    }
}
