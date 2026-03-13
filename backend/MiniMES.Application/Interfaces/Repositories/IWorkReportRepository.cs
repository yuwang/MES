using MiniMES.Domain.Entities;

namespace MiniMES.Application.Interfaces.Repositories;

/// <summary>
/// 报工记录仓储接口
/// </summary>
public interface IWorkReportRepository
{
    /// <summary>
    /// 获取所有报工记录（分页）
    /// </summary>
    Task<(List<WorkReport> Items, int Total)> GetPagedAsync(int page, int pageSize, long? workOrderId = null);

    /// <summary>
    /// 根据ID获取报工记录（包含工单、工位、报工人信息）
    /// </summary>
    Task<WorkReport?> GetByIdAsync(long id);

    /// <summary>
    /// 创建报工记录
    /// </summary>
    Task<WorkReport> CreateAsync(WorkReport workReport);

    /// <summary>
    /// 更新报工记录
    /// </summary>
    Task UpdateAsync(WorkReport workReport);

    /// <summary>
    /// 删除报工记录
    /// </summary>
    Task DeleteAsync(WorkReport workReport);
}
