using MiniMES.Domain.Entities;

namespace MiniMES.Application.Interfaces.Repositories;

/// <summary>
/// 工位仓储接口
/// </summary>
public interface IWorkstationRepository
{
    /// <summary>
    /// 获取所有工位（分页）
    /// </summary>
    Task<(List<Workstation> Items, int Total)> GetPagedAsync(int page, int pageSize, string? keyword = null);

    /// <summary>
    /// 根据ID获取工位
    /// </summary>
    Task<Workstation?> GetByIdAsync(long id);

    /// <summary>
    /// 根据工位编号获取工位
    /// </summary>
    Task<Workstation?> GetByCodeAsync(string stationCode);

    /// <summary>
    /// 创建工位
    /// </summary>
    Task<Workstation> CreateAsync(Workstation workstation);

    /// <summary>
    /// 更新工位
    /// </summary>
    Task UpdateAsync(Workstation workstation);

    /// <summary>
    /// 删除工位
    /// </summary>
    Task DeleteAsync(Workstation workstation);
}
