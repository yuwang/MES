using MiniMES.Domain.Enums;

namespace MiniMES.Domain.Entities;

public class WorkOrder : BaseEntity
{
    public string OrderNo { get; set; } = string.Empty;
    public long ProductId { get; set; }
    public int TargetQuantity { get; set; }
    public int CompletedQuantity { get; set; } = 0;
    public int DefectQuantity { get; set; } = 0;
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Pending;
    public long CreatedBy { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // 导航属性
    public Product? Product { get; set; }
    public User? Creator { get; set; }
    public ICollection<WorkReport> WorkReports { get; set; } = new List<WorkReport>();
}
