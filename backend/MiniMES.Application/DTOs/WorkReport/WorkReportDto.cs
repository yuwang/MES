namespace MiniMES.Application.DTOs.WorkReport;

/// <summary>
/// 报工记录响应DTO
/// </summary>
public class WorkReportDto
{
    /// <summary>
    /// 报工ID
    /// </summary>
    public long Id { get; set; }

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
    /// 工位ID
    /// </summary>
    public long WorkstationId { get; set; }

    /// <summary>
    /// 工位名称
    /// </summary>
    public string StationName { get; set; } = string.Empty;

    /// <summary>
    /// 良品数量
    /// </summary>
    public int GoodQuantity { get; set; }

    /// <summary>
    /// 不良品数量
    /// </summary>
    public int DefectQuantity { get; set; }

    /// <summary>
    /// 报工人ID
    /// </summary>
    public long ReportedBy { get; set; }

    /// <summary>
    /// 报工人姓名
    /// </summary>
    public string ReportedByName { get; set; } = string.Empty;

    /// <summary>
    /// 报工时间
    /// </summary>
    public DateTime ReportedAt { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
