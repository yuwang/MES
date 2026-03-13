namespace MiniMES.Domain.Entities;

public class OperationLog : BaseEntity
{
    public long UserId { get; set; }
    public string OperationType { get; set; } = string.Empty;
    public string? OperationDesc { get; set; }
    public string? IpAddress { get; set; }
}
