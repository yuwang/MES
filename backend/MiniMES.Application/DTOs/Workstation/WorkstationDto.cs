namespace MiniMES.Application.DTOs.Workstation;

/// <summary>
/// 工位响应DTO
/// </summary>
public class WorkstationDto
{
    /// <summary>
    /// 工位ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 工位编号
    /// </summary>
    public string StationCode { get; set; } = string.Empty;

    /// <summary>
    /// 工位名称
    /// </summary>
    public string StationName { get; set; } = string.Empty;

    /// <summary>
    /// 所属车间
    /// </summary>
    public string? Workshop { get; set; }

    /// <summary>
    /// 状态：1-启用 0-停用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
