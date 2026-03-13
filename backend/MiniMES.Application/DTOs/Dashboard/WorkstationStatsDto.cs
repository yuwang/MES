namespace MiniMES.Application.DTOs.Dashboard;

/// <summary>
/// 工位产能统计DTO
/// </summary>
public class WorkstationStatsDto
{
    /// <summary>
    /// 工位ID
    /// </summary>
    public long WorkstationId { get; set; }

    /// <summary>
    /// 工位名称
    /// </summary>
    public string StationName { get; set; } = string.Empty;

    /// <summary>
    /// 车间
    /// </summary>
    public string? Workshop { get; set; }

    /// <summary>
    /// 累计良品数
    /// </summary>
    public int TotalGoodQuantity { get; set; }

    /// <summary>
    /// 累计不良品数
    /// </summary>
    public int TotalDefectQuantity { get; set; }

    /// <summary>
    /// 报工次数
    /// </summary>
    public int ReportCount { get; set; }

    /// <summary>
    /// 良品率（百分比）
    /// </summary>
    public decimal QualityRate { get; set; }
}
