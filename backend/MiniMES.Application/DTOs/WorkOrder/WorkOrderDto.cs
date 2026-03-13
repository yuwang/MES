namespace MiniMES.Application.DTOs.WorkOrder;

/// <summary>
/// 工单响应DTO
/// </summary>
public class WorkOrderDto
{
    /// <summary>
    /// 工单ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 工单号
    /// </summary>
    public string OrderNo { get; set; } = string.Empty;

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
    /// 状态：0-待产 1-生产中 2-已完工 3-已取消
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    public long CreatedBy { get; set; }

    /// <summary>
    /// 创建人姓名
    /// </summary>
    public string CreatedByName { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 开工时间
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// 完工时间
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
