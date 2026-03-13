using System.ComponentModel.DataAnnotations;

namespace MiniMES.Application.DTOs.WorkReport;

/// <summary>
/// 更新报工记录请求DTO
/// </summary>
public class UpdateWorkReportDto
{
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
