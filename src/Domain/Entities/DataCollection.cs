using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// 数据采集实体
/// </summary>
public class DataCollection : BaseEntity
{
    /// <summary>
    /// 数据标题
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 采集时间
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// 采集内容
    /// </summary>
    [Required]
    public string Content { get; set; } = string.Empty;
}
