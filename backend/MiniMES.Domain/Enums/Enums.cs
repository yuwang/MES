namespace MiniMES.Domain.Enums;

public enum WorkOrderStatus
{
    Pending = 0,      // 待产
    InProgress = 1,   // 生产中
    Completed = 2,    // 已完工
    Cancelled = 3     // 已取消
}

public enum EntityStatus
{
    Disabled = 0,     // 停用
    Enabled = 1       // 启用
}
