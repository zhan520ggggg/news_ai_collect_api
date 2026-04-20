using Microsoft.AspNetCore.Authorization;
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
[Authorize]
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
    /// 批量创建采集数据（第三方调用，无需鉴权）
    /// </summary>
    /// <param name="dtos">采集数据数组，每项包含标题、时间和内容</param>
    /// <param name="ct">取消令牌</param>
    /// <returns>接收的数据条数</returns>
    /// <response code="200">接收成功</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(int), 200)]
    public async Task<IActionResult> Create(
        [FromBody] List<CreateDataCollectionDto> dtos,
        CancellationToken ct)
    {
        var count = await _dataService.CreateBatchAsync(dtos, ct);
        return Ok(new { count });
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
