using MiniMES.Domain.Enums;

namespace MiniMES.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public long RoleId { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Enabled;

    // 导航属性
    public Role? Role { get; set; }
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public ICollection<WorkReport> WorkReports { get; set; } = new List<WorkReport>();
}
