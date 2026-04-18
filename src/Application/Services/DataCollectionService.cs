using AutoMapper;
using Common.Models;
using Domain.Entities;
using Domain.Interfaces;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services;

/// <summary>
/// 数据采集服务实现
/// </summary>
public class DataCollectionService : IDataCollectionService
{
    private readonly IRepository<DataCollection> _dataRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DataCollectionService> _logger;

    public DataCollectionService(
        IRepository<DataCollection> dataRepository,
        IMapper mapper,
        ILogger<DataCollectionService> logger)
    {
        _dataRepository = dataRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DataCollectionResponseDto> CreateAsync(CreateDataCollectionDto dto, CancellationToken ct = default)
    {
        var data = _mapper.Map<DataCollection>(dto);
        data.Time = dto.Time ?? DateTime.UtcNow;

        var created = await _dataRepository.AddAsync(data, ct);

        _logger.LogInformation("数据采集成功：Title={Title}, Time={Time}",
            created.Title, created.Time);

        return _mapper.Map<DataCollectionResponseDto>(created);
    }

    public async Task<PagedResponse<DataCollectionResponseDto>> GetPagedAsync(
        PagedRequest request, CancellationToken ct = default)
    {
        var items = await _dataRepository.GetPagedAsync(
            request.PageNumber, request.PageSize, ct: ct);

        var total = await _dataRepository.CountAsync(ct: ct);

        return new PagedResponse<DataCollectionResponseDto>
        {
            Items = _mapper.Map<IReadOnlyList<DataCollectionResponseDto>>(items),
            TotalCount = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
