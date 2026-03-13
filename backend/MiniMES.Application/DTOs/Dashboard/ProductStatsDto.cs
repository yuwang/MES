namespace MiniMES.Application.DTOs.Dashboard;

/// <summary>
/// 产品产量统计DTO
/// </summary>
public class ProductStatsDto
{
    /// <summary>
    /// 产品ID
    /// </summary>
    public long ProductId { get; set; }

    /// <summary>
    /// 产品编码
    /// </summary>
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>
    /// 产品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 累计良品数
    /// </summary>
    public int TotalGoodQuantity { get; set; }

    /// <summary>
    /// 累计不良品数
    /// </summary>
    public int TotalDefectQuantity { get; set; }

    /// <summary>
    /// 工单数量
    /// </summary>
    public int WorkOrderCount { get; set; }

    /// <summary>
    /// 良品率（百分比）
    /// </summary>
    public decimal QualityRate { get; set; }
}
