using System.ComponentModel.DataAnnotations;

namespace MiniMES.Application.DTOs.Workstation;

/// <summary>
/// 创建工位请求DTO
/// </summary>
public class CreateWorkstationDto
{
    /// <summary>
    /// 工位编号
    /// </summary>
    [Required(ErrorMessage = "工位编号不能为空")]
    [StringLength(50, ErrorMessage = "工位编号长度不能超过50个字符")]
    public string StationCode { get; set; } = string.Empty;

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
}
