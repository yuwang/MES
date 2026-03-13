using Microsoft.EntityFrameworkCore;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Data;

namespace MiniMES.Infrastructure.Repositories;

/// <summary>
/// 工单仓储实现
/// </summary>
public class WorkOrderRepository : IWorkOrderRepository
{
    private readonly MiniMesDbContext _context;

    public WorkOrderRepository(MiniMesDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有工单（分页）
    /// </summary>
    public async Task<(List<WorkOrder> Items, int Total)> GetPagedAsync(int page, int pageSize, int? status = null, string? keyword = null)
    {
        var query = _context.WorkOrders
            .Include(w => w.Product)
            .Include(w => w.Creator)
            .AsNoTracking();

        // 状态筛选
        if (status.HasValue)
        {
            query = query.Where(w => (int)w.Status == status.Value);
        }

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(w => w.OrderNo.Contains(keyword) || w.Product.ProductName.Contains(keyword));
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
    /// 根据ID获取工单（包含产品和创建人信息）
    /// </summary>
    public async Task<WorkOrder?> GetByIdAsync(long id)
    {
        return await _context.WorkOrders
            .Include(w => w.Product)
            .Include(w => w.Creator)
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    /// <summary>
    /// 根据工单号获取工单
    /// </summary>
    public async Task<WorkOrder?> GetByOrderNoAsync(string orderNo)
    {
        return await _context.WorkOrders
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.OrderNo == orderNo);
    }

    /// <summary>
    /// 创建工单
    /// </summary>
    public async Task<WorkOrder> CreateAsync(WorkOrder workOrder)
    {
        await _context.WorkOrders.AddAsync(workOrder);
        await _context.SaveChangesAsync();
        return workOrder;
    }

    /// <summary>
    /// 更新工单
    /// </summary>
    public async Task UpdateAsync(WorkOrder workOrder)
    {
        _context.WorkOrders.Update(workOrder);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 删除工单
    /// </summary>
    public async Task DeleteAsync(WorkOrder workOrder)
    {
        _context.WorkOrders.Remove(workOrder);
        await _context.SaveChangesAsync();
    }
}
