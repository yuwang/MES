using MiniMES.Domain.Enums;

namespace MiniMES.Domain.Entities;

public class Product : BaseEntity
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Enabled;

    // 导航属性
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
}
