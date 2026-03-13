using MiniMES.Application.DTOs.WorkReport;
using MiniMES.Shared.Common;

namespace MiniMES.Application.Interfaces.Services;

/// <summary>
/// 报工服务接口
/// </summary>
public interface IWorkReportService
{
    /// <summary>
    /// 获取报工记录列表（分页）
    /// </summary>
    Task<ApiResponse<PagedResponse<WorkReportDto>>> GetPagedAsync(int page, int pageSize, long? workOrderId = null);

    /// <summary>
    /// 根据ID获取报工记录详情
    /// </summary>
    Task<ApiResponse<WorkReportDto>> GetByIdAsync(long id);

    /// <summary>
    /// 创建报工记录
    /// </summary>
    Task<ApiResponse<WorkReportDto>> CreateAsync(CreateWorkReportDto dto);

    /// <summary>
    /// 更新报工记录
    /// </summary>
    Task<ApiResponse<WorkReportDto>> UpdateAsync(long id, UpdateWorkReportDto dto);

    /// <summary>
    /// 删除报工记录
    /// </summary>
    Task<ApiResponse<bool>> DeleteAsync(long id);
}
