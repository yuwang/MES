namespace MiniMES.Domain.Entities;

public class WorkReport : BaseEntity
{
    public long WorkOrderId { get; set; }
    public long WorkstationId { get; set; }
    public int GoodQuantity { get; set; }
    public int DefectQuantity { get; set; } = 0;
    public long ReportedBy { get; set; }
    public DateTime ReportedAt { get; set; } = DateTime.Now;
    public string? Remark { get; set; }

    // 导航属性
    public WorkOrder? WorkOrder { get; set; }
    public Workstation? Workstation { get; set; }
    public User? Reporter { get; set; }
}
