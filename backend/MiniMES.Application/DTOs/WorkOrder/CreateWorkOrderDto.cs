using System.ComponentModel.DataAnnotations;

namespace MiniMES.Application.DTOs.WorkOrder;

/// <summary>
/// 创建工单请求DTO
/// </summary>
public class CreateWorkOrderDto
{
    /// <summary>
    /// 工单号
    /// </summary>
    [Required(ErrorMessage = "工单号不能为空")]
    [StringLength(50, ErrorMessage = "工单号长度不能超过50个字符")]
    public string OrderNo { get; set; } = string.Empty;

    /// <summary>
    /// 产品ID
    /// </summary>
    [Required(ErrorMessage = "产品ID不能为空")]
    [Range(1, long.MaxValue, ErrorMessage = "产品ID必须大于0")]
    public long ProductId { get; set; }

    /// <summary>
    /// 目标产量
    /// </summary>
    [Required(ErrorMessage = "目标产量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "目标产量必须大于0")]
    public int TargetQuantity { get; set; }
}
