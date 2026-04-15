using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// 产品实体
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// 产品名称
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 产品描述
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// 价格
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "价格必须大于 0")]
    public decimal Price { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    [MaxLength(100)]
    public string? Category { get; set; }

    /// <summary>
    /// 是否上架
    /// </summary>
    public bool IsPublished { get; set; }
}
