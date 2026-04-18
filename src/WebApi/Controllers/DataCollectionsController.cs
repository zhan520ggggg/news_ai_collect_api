using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using Common.Models;

namespace WebApi.Controllers;

/// <summary>
/// 数据采集接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DataCollectionsController : ControllerBase
{
    private readonly IDataCollectionService _dataService;
    private readonly ILogger<DataCollectionsController> _logger;

    public DataCollectionsController(
        IDataCollectionService dataService,
        ILogger<DataCollectionsController> logger)
    {
        _dataService = dataService;
        _logger = logger;
    }

    /// <summary>
    /// 创建采集数据
    /// </summary>
    /// <param name="dto">采集数据信息，包含标题、时间和内容</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>创建的采集数据信息</returns>
    /// <response code="200">创建成功</response>
    [HttpPost]
    [ProducesResponseType(typeof(DataCollectionResponseDto), 200)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDataCollectionDto dto,
        CancellationToken ct)
    {
        _logger.LogInformation("接收数据采集请求：Title={Title}", dto.Title);
        var result = await _dataService.CreateAsync(dto, ct);
        return Ok(result);
    }

    /// <summary>
    /// 分页获取采集数据
    /// </summary>
    /// <param name="request">分页参数</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>分页数据列表</returns>
    /// <response code="200">查询成功</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<DataCollectionResponseDto>), 200)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] PagedRequest request,
        CancellationToken ct)
    {
        var result = await _dataService.GetPagedAsync(request, ct);
        return Ok(result);
    }
}
