using MiniMES.Domain.Enums;

namespace MiniMES.Domain.Entities;

public class Workstation : BaseEntity
{
    public string StationCode { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public string? Workshop { get; set; }
    public EntityStatus Status { get; set; } = EntityStatus.Enabled;

    // 导航属性
    public ICollection<WorkReport> WorkReports { get; set; } = new List<WorkReport>();
}
