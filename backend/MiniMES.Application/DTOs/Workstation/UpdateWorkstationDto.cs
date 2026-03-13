using System.ComponentModel.DataAnnotations;

namespace MiniMES.Application.DTOs.Workstation;

/// <summary>
/// 更新工位请求DTO
/// </summary>
public class UpdateWorkstationDto
{
    /// <summary>
    /// 工位名称
    /// </summary>
    [Required(ErrorMessage = "工位名称不能为空")]
    [StringLength(100, ErrorMessage = "工位名称长度不能超过100个字符")]
    public string StationName { get; set; } = string.Empty;

    /// <summary>
    /// 所属车间
    /// </summary>
    [StringLength(100, ErrorMessage = "车间名称长度不能超过100个字符")]
    public string? Workshop { get; set; }

    /// <summary>
    /// 状态：1-启用 0-停用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; }
}
