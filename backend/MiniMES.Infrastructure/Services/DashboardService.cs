using Microsoft.EntityFrameworkCore;
using MiniMES.Application.DTOs.Dashboard;
using MiniMES.Application.Interfaces.Services;
using MiniMES.Domain.Enums;
using MiniMES.Infrastructure.Data;
using MiniMES.Shared.Common;

namespace MiniMES.Infrastructure.Services;

/// <summary>
/// 生产看板服务实现
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly MiniMesDbContext _context;

    public DashboardService(MiniMesDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取生产进度监控数据（所有进行中的工单）
    /// </summary>
    public async Task<ApiResponse<List<ProductionProgressDto>>> GetProductionProgressAsync()
    {
        var workOrders = await _context.WorkOrders
            .Include(w => w.Product)
            .Where(w => w.Status == WorkOrderStatus.InProgress || w.Status == WorkOrderStatus.Pending)
            .AsNoTracking()
            .OrderBy(w => w.CreatedAt)
            .ToListAsync();

        var progressList = workOrders.Select(w =>
        {
            var totalQuantity = w.CompletedQuantity + w.DefectQuantity;
            var completionRate = w.TargetQuantity > 0 ? (decimal)w.CompletedQuantity / w.TargetQuantity * 100 : 0;
            var qualityRate = totalQuantity > 0 ? (decimal)w.CompletedQuantity / totalQuantity * 100 : 0;

            return new ProductionProgressDto
            {
                WorkOrderId = w.Id,
                OrderNo = w.OrderNo,
                ProductName = w.Product.ProductName,
                TargetQuantity = w.TargetQuantity,
                CompletedQuantity = w.CompletedQuantity,
                DefectQuantity = w.DefectQuantity,
                CompletionRate = Math.Round(completionRate, 2),
                QualityRate = Math.Round(qualityRate, 2),
                Status = (int)w.Status,
                StartedAt = w.StartedAt
            };
        }).ToList();

        return ApiResponse<List<ProductionProgressDto>>.SuccessResult(progressList);
    }

    /// <summary>
    /// 获取工位产能统计
    /// </summary>
    public async Task<ApiResponse<List<WorkstationStatsDto>>> GetWorkstationStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.WorkReports
            .Include(r => r.Workstation)
            .AsNoTracking();

        // 时间范围筛选
        if (startDate.HasValue)
        {
            query = query.Where(r => r.ReportedAt >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(r => r.ReportedAt <= endDate.Value);
        }

        var stats = await query
            .GroupBy(r => new { r.WorkstationId, r.Workstation.StationName, r.Workstation.Workshop })
            .Select(g => new
            {
                g.Key.WorkstationId,
                g.Key.StationName,
                g.Key.Workshop,
                TotalGoodQuantity = g.Sum(r => r.GoodQuantity),
                TotalDefectQuantity = g.Sum(r => r.DefectQuantity),
                ReportCount = g.Count()
            })
            .ToListAsync();

        var statsList = stats.Select(s =>
        {
            var totalQuantity = s.TotalGoodQuantity + s.TotalDefectQuantity;
            var qualityRate = totalQuantity > 0 ? (decimal)s.TotalGoodQuantity / totalQuantity * 100 : 0;

            return new WorkstationStatsDto
            {
                WorkstationId = s.WorkstationId,
                StationName = s.StationName,
                Workshop = s.Workshop,
                TotalGoodQuantity = s.TotalGoodQuantity,
                TotalDefectQuantity = s.TotalDefectQuantity,
                ReportCount = s.ReportCount,
                QualityRate = Math.Round(qualityRate, 2)
            };
        }).OrderByDescending(s => s.TotalGoodQuantity).ToList();

        return ApiResponse<List<WorkstationStatsDto>>.SuccessResult(statsList);
    }

    /// <summary>
    /// 获取产品产量统计
    /// </summary>
    public async Task<ApiResponse<List<ProductStatsDto>>> GetProductStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.WorkOrders
            .Include(w => w.Product)
            .AsNoTracking();

        // 时间范围筛选
        if (startDate.HasValue)
        {
            query = query.Where(w => w.CreatedAt >= startDate.Value);
        }
        if (endDate.HasValue)
        {
            query = query.Where(w => w.CreatedAt <= endDate.Value);
        }

        var stats = await query
            .GroupBy(w => new { w.ProductId, w.Product.ProductCode, w.Product.ProductName })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ProductCode,
                g.Key.ProductName,
                TotalGoodQuantity = g.Sum(w => w.CompletedQuantity),
                TotalDefectQuantity = g.Sum(w => w.DefectQuantity),
                WorkOrderCount = g.Count()
            })
            .ToListAsync();

        var statsList = stats.Select(s =>
        {
            var totalQuantity = s.TotalGoodQuantity + s.TotalDefectQuantity;
            var qualityRate = totalQuantity > 0 ? (decimal)s.TotalGoodQuantity / totalQuantity * 100 : 0;

            return new ProductStatsDto
            {
                ProductId = s.ProductId,
                ProductCode = s.ProductCode,
                ProductName = s.ProductName,
                TotalGoodQuantity = s.TotalGoodQuantity,
                TotalDefectQuantity = s.TotalDefectQuantity,
                WorkOrderCount = s.WorkOrderCount,
                QualityRate = Math.Round(qualityRate, 2)
            };
        }).OrderByDescending(s => s.TotalGoodQuantity).ToList();

        return ApiResponse<List<ProductStatsDto>>.SuccessResult(statsList);
    }
}
