using MiniMES.Application.DTOs.Workstation;
using MiniMES.Shared.Common;

namespace MiniMES.Application.Interfaces.Services;

/// <summary>
/// 工位服务接口
/// </summary>
public interface IWorkstationService
{
    /// <summary>
    /// 获取工位列表（分页）
    /// </summary>
    Task<ApiResponse<PagedResponse<WorkstationDto>>> GetPagedAsync(int page, int pageSize, string? keyword = null);

    /// <summary>
    /// 根据ID获取工位详情
    /// </summary>
    Task<ApiResponse<WorkstationDto>> GetByIdAsync(long id);

    /// <summary>
    /// 创建工位
    /// </summary>
    Task<ApiResponse<WorkstationDto>> CreateAsync(CreateWorkstationDto dto);

    /// <summary>
    /// 更新工位
    /// </summary>
    Task<ApiResponse<WorkstationDto>> UpdateAsync(long id, UpdateWorkstationDto dto);

    /// <summary>
    /// 删除工位
    /// </summary>
    Task<ApiResponse<bool>> DeleteAsync(long id);
}
