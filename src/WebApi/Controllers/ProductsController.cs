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
    /// 创建产品
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var result = await _productService.CreateProductAsync(dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 根据 ID 获取产品
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetProduct(Guid id, CancellationToken ct)
    {
        var result = await _productService.GetProductByIdAsync(id, ct);
        return Ok(result);
    }

    /// <summary>
    /// 获取所有产品
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ProductResponseDto>>), 200)]
    public async Task<IActionResult> GetAllProducts(CancellationToken ct)
    {
        var result = await _productService.GetAllProductsAsync(ct);
        return Ok(result);
    }

    /// <summary>
    /// 分页获取产品
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ProductResponseDto>>), 200)]
    public async Task<IActionResult> GetProductsPaged(
        [FromQuery] PagedRequest request, CancellationToken ct)
    {
        var result = await _productService.GetProductsPagedAsync(request, ct);
        return Ok(result);
    }

    /// <summary>
    /// 更新产品
    /// </summary>
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
    /// 删除产品
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken ct)
    {
        await _productService.DeleteProductAsync(id, ct);
        return Ok(new { message = "产品删除成功" });
    }
}
