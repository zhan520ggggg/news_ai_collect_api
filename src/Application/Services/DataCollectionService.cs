using AutoMapper;
using Common.Models;
using Domain.Entities;
using Domain.Interfaces;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

/// <summary>
/// 数据采集服务实现
/// </summary>
public class DataCollectionService : IDataCollectionService
{
    private readonly IRepository<DataCollection> _dataRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DataCollectionService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DataCollectionService(
        IRepository<DataCollection> dataRepository,
        IMapper mapper,
        ILogger<DataCollectionService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _dataRepository = dataRepository;
        _mapper = mapper;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
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

    public Task<int> CreateBatchAsync(
        IEnumerable<CreateDataCollectionDto> dtos, CancellationToken ct = default)
    {
        var dtoList = dtos.ToList();
        var count = dtoList.Count;

        _logger.LogInformation("接收批量数据采集请求：Count={Count}", count);

        // 快速返回，后台处理数据持久化
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IRepository<DataCollection>>();

                var entities = dtoList.Select(dto =>
                {
                    var entity = _mapper.Map<DataCollection>(dto);
                    entity.Time = dto.Time ?? DateTime.UtcNow;
                    return entity;
                }).ToList();

                await repository.AddRangeAsync(entities, CancellationToken.None);

                _logger.LogInformation("批量数据采集持久化成功：Count={Count}", count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "批量数据采集持久化失败：Count={Count}", count);
            }
        }, CancellationToken.None);

        return Task.FromResult(count);
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
