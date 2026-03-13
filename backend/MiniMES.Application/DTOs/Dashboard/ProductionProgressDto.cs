namespace MiniMES.Application.DTOs.Dashboard;

/// <summary>
/// 生产进度统计DTO
/// </summary>
public class ProductionProgressDto
{
    /// <summary>
    /// 工单ID
    /// </summary>
    public long WorkOrderId { get; set; }

    /// <summary>
    /// 工单号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 产品名称
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 目标产量
    /// </summary>
    public int TargetQuantity { get; set; }

    /// <summary>
    /// 累计良品数
    /// </summary>
    public int CompletedQuantity { get; set; }

    /// <summary>
    /// 累计不良品数
    /// </summary>
    public int DefectQuantity { get; set; }

    /// <summary>
    /// 完成率（百分比）
    /// </summary>
    public decimal CompletionRate { get; set; }

    /// <summary>
    /// 良品率（百分比）
    /// </summary>
    public decimal QualityRate { get; set; }

    /// <summary>
    /// 工单状态：0-待产 1-生产中 2-已完工 3-已取消
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 开工时间
    /// </summary>
    public DateTime? StartedAt { get; set; }
}
