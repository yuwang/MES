using System.ComponentModel.DataAnnotations;

namespace MiniMES.Application.DTOs.WorkOrder;

/// <summary>
/// 更新工单请求DTO
/// </summary>
public class UpdateWorkOrderDto
{
    /// <summary>
    /// 目标产量
    /// </summary>
    [Required(ErrorMessage = "目标产量不能为空")]
    [Range(1, int.MaxValue, ErrorMessage = "目标产量必须大于0")]
    public int TargetQuantity { get; set; }
}
