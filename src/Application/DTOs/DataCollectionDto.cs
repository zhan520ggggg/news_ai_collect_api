namespace Application.DTOs;

/// <summary>
/// 创建数据采集请求
/// </summary>
public class CreateDataCollectionDto
{
    /// <summary>
    /// 数据标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 采集时间
    /// </summary>
    public DateTime? Time { get; set; }

    /// <summary>
    /// 采集内容
    /// </summary>
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// 数据采集响应
/// </summary>
public class DataCollectionResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
