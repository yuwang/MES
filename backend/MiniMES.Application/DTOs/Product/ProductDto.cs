namespace MiniMES.Application.DTOs.Product;

/// <summary>
/// 产品响应DTO
/// </summary>
public class ProductDto
{
    /// <summary>
    /// 产品ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 产品编码
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// 产品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 规格型号
    /// </summary>
    public string? Specification { get; set; }

    /// <summary>
    /// 状态：1-启用 0-停用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
