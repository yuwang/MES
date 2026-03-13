using MiniMES.Application.DTOs.Dashboard;
using MiniMES.Shared.Common;

namespace MiniMES.Application.Interfaces.Services;

/// <summary>
/// 生产看板服务接口
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// 获取生产进度监控数据
    /// </summary>
    Task<ApiResponse<List<ProductionProgressDto>>> GetProductionProgressAsync();

    /// <summary>
    /// 获取工位产能统计
    /// </summary>
    Task<ApiResponse<List<WorkstationStatsDto>>> GetWorkstationStatsAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// 获取产品产量统计
    /// </summary>
    Task<ApiResponse<List<ProductStatsDto>>> GetProductStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
}
