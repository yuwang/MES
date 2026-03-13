using System.ComponentModel.DataAnnotations;

namespace MiniMES.Application.DTOs.WorkReport;

/// <summary>
/// 创建报工记录请求DTO
/// </summary>
public class CreateWorkReportDto
{
    /// <summary>
    /// 工单ID
    /// </summary>
    [Required(ErrorMessage = "工单ID不能为空")]
    [Range(1, long.MaxValue, ErrorMessage = "工单ID必须大于0")]
    public long WorkOrderId { get; set; }

    /// <summary>
    /// 工位ID
    /// </summary>
    [Required(ErrorMessage = "工位ID不能为空")]
    [Range(1, long.MaxValue, ErrorMessage = "工位ID必须大于0")]
    public long WorkstationId { get; set; }

    /// <summary>
    /// 良品数量
    /// </summary>
    [Required(ErrorMessage = "良品数量不能为空")]
    [Range(0, int.MaxValue, ErrorMessage = "良品数量不能为负数")]
    public int GoodQuantity { get; set; }

    /// <summary>
    /// 不良品数量
    /// </summary>
    [Required(ErrorMessage = "不良品数量不能为空")]
    [Range(0, int.MaxValue, ErrorMessage = "不良品数量不能为负数")]
    public int DefectQuantity { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [StringLength(500, ErrorMessage = "备注长度不能超过500个字符")]
    public string? Remark { get; set; }
}
