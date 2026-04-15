using AutoMapper;
using Common.Models;
using Domain.Interfaces;
using Application.DTOs;
using Application.Interfaces;

namespace Application.Services;

/// <summary>
/// 产品服务实现
/// </summary>
public class ProductService : IProductService
{
    // IRepository 基类够用， 所以不用实现该仓储
    private readonly IRepository<Domain.Entities.Product> _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IRepository<Domain.Entities.Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductResponseDto> CreateProductAsync(
        CreateProductDto dto, CancellationToken ct = default)
    {
        var product = _mapper.Map<Domain.Entities.Product>(dto);
        var created = await _productRepository.AddAsync(product, ct);
        return _mapper.Map<ProductResponseDto>(created);
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(
        Guid id, CancellationToken ct = default)
    {
        var product = await _productRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), id);

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<IReadOnlyList<ProductResponseDto>> GetAllProductsAsync(
        CancellationToken ct = default)
    {
        var products = await _productRepository.GetAllAsync(ct);
        return _mapper.Map<IReadOnlyList<ProductResponseDto>>(products);
    }

    public async Task<PagedResponse<ProductResponseDto>> GetProductsPagedAsync(
        PagedRequest request, CancellationToken ct = default)
    {
        var items = await _productRepository.GetPagedAsync(
            request.PageNumber, request.PageSize, ct: ct);

        var total = await _productRepository.CountAsync(ct: ct);

        return new PagedResponse<ProductResponseDto>
        {
            Items = _mapper.Map<IReadOnlyList<ProductResponseDto>>(items),
            TotalCount = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<ProductResponseDto> UpdateProductAsync(
        Guid id, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _productRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), id);

        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Description != null) product.Description = dto.Description;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.Stock.HasValue) product.Stock = dto.Stock.Value;
        if (dto.Category != null) product.Category = dto.Category;
        if (dto.IsPublished.HasValue) product.IsPublished = dto.IsPublished.Value;

        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product, ct);
        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken ct = default)
    {
        var product = await _productRepository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), id);

        await _productRepository.DeleteAsync(product, ct);
    }
}
