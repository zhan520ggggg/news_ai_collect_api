using Application.DTOs;
using Common.Models;

namespace Application.Interfaces;

/// <summary>
/// 数据采集服务接口
/// </summary>
public interface IDataCollectionService
{
    /// <summary>
    /// 创建采集数据
    /// </summary>
    Task<DataCollectionResponseDto> CreateAsync(CreateDataCollectionDto dto, CancellationToken ct = default);

    /// <summary>
    /// 分页获取采集数据
    /// </summary>
    Task<PagedResponse<DataCollectionResponseDto>> GetPagedAsync(PagedRequest request, CancellationToken ct = default);
}
