using Microsoft.EntityFrameworkCore;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Data;

namespace MiniMES.Infrastructure.Repositories;

/// <summary>
/// 工位仓储实现
/// </summary>
public class WorkstationRepository : IWorkstationRepository
{
    private readonly MiniMesDbContext _context;

    public WorkstationRepository(MiniMesDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有工位（分页）
    /// </summary>
    public async Task<(List<Workstation> Items, int Total)> GetPagedAsync(int page, int pageSize, string? keyword = null)
    {
        var query = _context.Workstations.AsNoTracking();

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(w => w.StationCode.Contains(keyword) || w.StationName.Contains(keyword));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(w => w.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    /// <summary>
    /// 根据ID获取工位
    /// </summary>
    public async Task<Workstation?> GetByIdAsync(long id)
    {
        return await _context.Workstations
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    /// <summary>
    /// 根据工位编号获取工位
    /// </summary>
    public async Task<Workstation?> GetByCodeAsync(string stationCode)
    {
        return await _context.Workstations
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.StationCode == stationCode);
    }

    /// <summary>
    /// 创建工位
    /// </summary>
    public async Task<Workstation> CreateAsync(Workstation workstation)
    {
        await _context.Workstations.AddAsync(workstation);
        await _context.SaveChangesAsync();
        return workstation;
    }

    /// <summary>
    /// 更新工位
    /// </summary>
    public async Task UpdateAsync(Workstation workstation)
    {
        _context.Workstations.Update(workstation);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 删除工位
    /// </summary>
    public async Task DeleteAsync(Workstation workstation)
    {
        _context.Workstations.Remove(workstation);
        await _context.SaveChangesAsync();
    }
}
