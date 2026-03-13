using Microsoft.EntityFrameworkCore;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Data;

namespace MiniMES.Infrastructure.Repositories;

/// <summary>
/// 报工记录仓储实现
/// </summary>
public class WorkReportRepository : IWorkReportRepository
{
    private readonly MiniMesDbContext _context;

    public WorkReportRepository(MiniMesDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有报工记录（分页）
    /// </summary>
    public async Task<(List<WorkReport> Items, int Total)> GetPagedAsync(int page, int pageSize, long? workOrderId = null)
    {
        var query = _context.WorkReports
            .Include(r => r.WorkOrder)
                .ThenInclude(w => w.Product)
            .Include(r => r.Workstation)
            .Include(r => r.Reporter)
            .AsNoTracking();

        // 工单筛选
        if (workOrderId.HasValue)
        {
            query = query.Where(r => r.WorkOrderId == workOrderId.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.ReportedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    /// <summary>
    /// 根据ID获取报工记录（包含工单、工位、报工人信息）
    /// </summary>
    public async Task<WorkReport?> GetByIdAsync(long id)
    {
        return await _context.WorkReports
            .Include(r => r.WorkOrder)
                .ThenInclude(w => w.Product)
            .Include(r => r.Workstation)
            .Include(r => r.Reporter)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    /// <summary>
    /// 创建报工记录
    /// </summary>
    public async Task<WorkReport> CreateAsync(WorkReport workReport)
    {
        await _context.WorkReports.AddAsync(workReport);
        await _context.SaveChangesAsync();
        return workReport;
    }

    /// <summary>
    /// 更新报工记录
    /// </summary>
    public async Task UpdateAsync(WorkReport workReport)
    {
        _context.WorkReports.Update(workReport);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 删除报工记录
    /// </summary>
    public async Task DeleteAsync(WorkReport workReport)
    {
        _context.WorkReports.Remove(workReport);
        await _context.SaveChangesAsync();
    }
}
