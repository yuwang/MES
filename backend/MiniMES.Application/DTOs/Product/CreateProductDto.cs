using System.ComponentModel.DataAnnotations;

namespace MiniMES.Application.DTOs.Product;

/// <summary>
/// 创建产品请求DTO
/// </summary>
public class CreateProductDto
{
    /// <summary>
    /// 产品编码
    /// </summary>
    [Required(ErrorMessage = "产品编码不能为空")]
    [StringLength(50, ErrorMessage = "产品编码长度不能超过50个字符")]
    public string ProductCode { get; set; } = string.Empty;

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
}
