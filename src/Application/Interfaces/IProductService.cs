using Application.DTOs;
using Common.Models;

namespace Application.Interfaces;

/// <summary>
/// 产品服务接口
/// </summary>
public interface IProductService
{
    /// <summary>
    /// 创建产品
    /// </summary>
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto, CancellationToken ct = default);

    /// <summary>
    /// 根据 ID 获取产品
    /// </summary>
    Task<ProductResponseDto> GetProductByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// 获取所有产品
    /// </summary>
    Task<IReadOnlyList<ProductResponseDto>> GetAllProductsAsync(CancellationToken ct = default);

    /// <summary>
    /// 分页获取产品
    /// </summary>
    Task<PagedResponse<ProductResponseDto>> GetProductsPagedAsync(
        PagedRequest request, CancellationToken ct = default);

    /// <summary>
    /// 更新产品
    /// </summary>
    Task<ProductResponseDto> UpdateProductAsync(
        Guid id, UpdateProductDto dto, CancellationToken ct = default);

    /// <summary>
    /// 删除产品
    /// </summary>
    Task DeleteProductAsync(Guid id, CancellationToken ct = default);
}
