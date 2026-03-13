using System.ComponentModel.DataAnnotations;

namespace MiniMES.Application.DTOs.Product;

/// <summary>
/// 更新产品请求DTO
/// </summary>
public class UpdateProductDto
{
    /// <summary>
    /// 产品名称
    /// </summary>
    [Required(ErrorMessage = "产品名称不能为空")]
    [StringLength(100, ErrorMessage = "产品名称长度不能超过100个字符")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 规格型号
    /// </summary>
    [StringLength(200, ErrorMessage = "规格型号长度不能超过200个字符")]
    public string? Specification { get; set; }

    /// <summary>
    /// 状态：1-启用 0-停用
    /// </summary>
    [Range(0, 1, ErrorMessage = "状态值必须为0或1")]
    public int Status { get; set; }
}
