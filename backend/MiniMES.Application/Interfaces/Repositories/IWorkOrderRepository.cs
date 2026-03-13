using MiniMES.Domain.Entities;

namespace MiniMES.Application.Interfaces.Repositories;

/// <summary>
/// 工单仓储接口
/// </summary>
public interface IWorkOrderRepository
{
    /// <summary>
    /// 获取所有工单（分页）
    /// </summary>
    Task<(List<WorkOrder> Items, int Total)> GetPagedAsync(int page, int pageSize, int? status = null, string? keyword = null);

    /// <summary>
    /// 根据ID获取工单（包含产品和创建人信息）
    /// </summary>
    Task<WorkOrder?> GetByIdAsync(long id);

    /// <summary>
    /// 根据工单号获取工单
    /// </summary>
    Task<WorkOrder?> GetByOrderNoAsync(string orderNo);

    /// <summary>
    /// 创建工单
    /// </summary>
    Task<WorkOrder> CreateAsync(WorkOrder workOrder);

    /// <summary>
    /// 更新工单
    /// </summary>
    Task UpdateAsync(WorkOrder workOrder);

    /// <summary>
    /// 删除工单
    /// </summary>
    Task DeleteAsync(WorkOrder workOrder);
}
