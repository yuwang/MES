using MiniMES.Application.DTOs.WorkOrder;
using MiniMES.Shared.Common;

namespace MiniMES.Application.Interfaces.Services;

/// <summary>
/// 工单服务接口
/// </summary>
public interface IWorkOrderService
{
    /// <summary>
    /// 获取工单列表（分页）
    /// </summary>
    Task<ApiResponse<PagedResponse<WorkOrderDto>>> GetPagedAsync(int page, int pageSize, int? status = null, string? keyword = null);

    /// <summary>
    /// 根据ID获取工单详情
    /// </summary>
    Task<ApiResponse<WorkOrderDto>> GetByIdAsync(long id);

    /// <summary>
    /// 创建工单
    /// </summary>
    Task<ApiResponse<WorkOrderDto>> CreateAsync(CreateWorkOrderDto dto);

    /// <summary>
    /// 更新工单
    /// </summary>
    Task<ApiResponse<WorkOrderDto>> UpdateAsync(long id, UpdateWorkOrderDto dto);

    /// <summary>
    /// 取消工单
    /// </summary>
    Task<ApiResponse<WorkOrderDto>> CancelAsync(long id);

    /// <summary>
    /// 删除工单
    /// </summary>
    Task<ApiResponse<bool>> DeleteAsync(long id);
}
