using MiniMES.Application.DTOs.WorkReport;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Application.Interfaces.Services;
using MiniMES.Domain.Entities;
using MiniMES.Domain.Enums;
using MiniMES.Shared.Common;

namespace MiniMES.Infrastructure.Services;

/// <summary>
/// 报工服务实现
/// </summary>
public class WorkReportService : IWorkReportService
{
    private readonly IWorkReportRepository _workReportRepository;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IWorkstationRepository _workstationRepository;
    private readonly ICurrentUserService _currentUserService;

    public WorkReportService(
        IWorkReportRepository workReportRepository,
        IWorkOrderRepository workOrderRepository,
        IWorkstationRepository workstationRepository,
        ICurrentUserService currentUserService)
    {
        _workReportRepository = workReportRepository;
        _workOrderRepository = workOrderRepository;
        _workstationRepository = workstationRepository;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// 获取报工记录列表（分页）
    /// </summary>
    public async Task<ApiResponse<PagedResponse<WorkReportDto>>> GetPagedAsync(int page, int pageSize, long? workOrderId = null)
    {
        var (items, total) = await _workReportRepository.GetPagedAsync(page, pageSize, workOrderId);

        var workReportDtos = items.Select(r => new WorkReportDto
        {
            Id = r.Id,
            WorkOrderId = r.WorkOrderId,
            OrderNo = r.WorkOrder.OrderNo,
            ProductName = r.WorkOrder.Product.ProductName,
            WorkstationId = r.WorkstationId,
            StationName = r.Workstation.StationName,
            GoodQuantity = r.GoodQuantity,
            DefectQuantity = r.DefectQuantity,
            ReportedBy = r.ReportedBy,
            ReportedByName = r.Reporter.RealName,
            ReportedAt = r.ReportedAt,
            Remark = r.Remark,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        }).ToList();

        var pagedResponse = new PagedResponse<WorkReportDto>
        {
            Items = workReportDtos,
            Total = total,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponse<PagedResponse<WorkReportDto>>.SuccessResult(pagedResponse);
    }

    /// <summary>
    /// 根据ID获取报工记录详情
    /// </summary>
    public async Task<ApiResponse<WorkReportDto>> GetByIdAsync(long id)
    {
        var workReport = await _workReportRepository.GetByIdAsync(id);
        if (workReport == null)
        {
            return ApiResponse<WorkReportDto>.FailResult("报工记录不存在");
        }

        var workReportDto = new WorkReportDto
        {
            Id = workReport.Id,
            WorkOrderId = workReport.WorkOrderId,
            OrderNo = workReport.WorkOrder.OrderNo,
            ProductName = workReport.WorkOrder.Product.ProductName,
            WorkstationId = workReport.WorkstationId,
            StationName = workReport.Workstation.StationName,
            GoodQuantity = workReport.GoodQuantity,
            DefectQuantity = workReport.DefectQuantity,
            ReportedBy = workReport.ReportedBy,
            ReportedByName = workReport.Reporter.RealName,
            ReportedAt = workReport.ReportedAt,
            Remark = workReport.Remark,
            CreatedAt = workReport.CreatedAt,
            UpdatedAt = workReport.UpdatedAt
        };

        return ApiResponse<WorkReportDto>.SuccessResult(workReportDto);
    }

    /// <summary>
    /// 创建报工记录
    /// </summary>
    public async Task<ApiResponse<WorkReportDto>> CreateAsync(CreateWorkReportDto dto)
    {
        // 检查工单是否存在
        var workOrder = await _workOrderRepository.GetByIdAsync(dto.WorkOrderId);
        if (workOrder == null)
        {
            return ApiResponse<WorkReportDto>.FailResult("工单不存在");
        }

        // 检查工单状态（只有待产和生产中的工单才能报工）
        if (workOrder.Status != WorkOrderStatus.Pending && workOrder.Status != WorkOrderStatus.InProgress)
        {
            return ApiResponse<WorkReportDto>.FailResult("只有待产或生产中的工单才能报工");
        }

        // 检查工位是否存在
        var workstation = await _workstationRepository.GetByIdAsync(dto.WorkstationId);
        if (workstation == null)
        {
            return ApiResponse<WorkReportDto>.FailResult("工位不存在");
        }

        // 检查工位是否启用
        if (workstation.Status == EntityStatus.Disabled)
        {
            return ApiResponse<WorkReportDto>.FailResult("工位已停用，无法报工");
        }

        // 检查报工数量
        if (dto.GoodQuantity == 0 && dto.DefectQuantity == 0)
        {
            return ApiResponse<WorkReportDto>.FailResult("良品数量和不良品数量不能同时为0");
        }

        var currentUserId = _currentUserService.GetUserId();
        var workReport = new WorkReport
        {
            WorkOrderId = dto.WorkOrderId,
            WorkstationId = dto.WorkstationId,
            GoodQuantity = dto.GoodQuantity,
            DefectQuantity = dto.DefectQuantity,
            ReportedBy = currentUserId,
            ReportedAt = DateTime.UtcNow,
            Remark = dto.Remark,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdWorkReport = await _workReportRepository.CreateAsync(workReport);

        // 更新工单的累计数量
        workOrder.CompletedQuantity += dto.GoodQuantity;
        workOrder.DefectQuantity += dto.DefectQuantity;

        // 如果工单是待产状态，自动转为生产中
        if (workOrder.Status == WorkOrderStatus.Pending)
        {
            workOrder.Status = WorkOrderStatus.InProgress;
            workOrder.StartedAt = DateTime.UtcNow;
        }

        // 如果累计良品数达到目标产量，自动完工
        if (workOrder.CompletedQuantity >= workOrder.TargetQuantity)
        {
            workOrder.Status = WorkOrderStatus.Completed;
            workOrder.CompletedAt = DateTime.UtcNow;
        }

        workOrder.UpdatedAt = DateTime.UtcNow;
        await _workOrderRepository.UpdateAsync(workOrder);

        // 重新查询以获取关联数据
        var workReportWithDetails = await _workReportRepository.GetByIdAsync(createdWorkReport.Id);

        var workReportDto = new WorkReportDto
        {
            Id = workReportWithDetails!.Id,
            WorkOrderId = workReportWithDetails.WorkOrderId,
            OrderNo = workReportWithDetails.WorkOrder.OrderNo,
            ProductName = workReportWithDetails.WorkOrder.Product.ProductName,
            WorkstationId = workReportWithDetails.WorkstationId,
            StationName = workReportWithDetails.Workstation.StationName,
            GoodQuantity = workReportWithDetails.GoodQuantity,
            DefectQuantity = workReportWithDetails.DefectQuantity,
            ReportedBy = workReportWithDetails.ReportedBy,
            ReportedByName = workReportWithDetails.Reporter.RealName,
            ReportedAt = workReportWithDetails.ReportedAt,
            Remark = workReportWithDetails.Remark,
            CreatedAt = workReportWithDetails.CreatedAt,
            UpdatedAt = workReportWithDetails.UpdatedAt
        };

        return ApiResponse<WorkReportDto>.SuccessResult(workReportDto, "报工成功");
    }

    /// <summary>
    /// 更新报工记录
    /// </summary>
    public async Task<ApiResponse<WorkReportDto>> UpdateAsync(long id, UpdateWorkReportDto dto)
    {
        var workReport = await _workReportRepository.GetByIdAsync(id);
        if (workReport == null)
        {
            return ApiResponse<WorkReportDto>.FailResult("报工记录不存在");
        }

        // 检查报工数量
        if (dto.GoodQuantity == 0 && dto.DefectQuantity == 0)
        {
            return ApiResponse<WorkReportDto>.FailResult("良品数量和不良品数量不能同时为0");
        }

        // 计算数量差异
        var goodDiff = dto.GoodQuantity - workReport.GoodQuantity;
        var defectDiff = dto.DefectQuantity - workReport.DefectQuantity;

        // 更新报工记录
        workReport.GoodQuantity = dto.GoodQuantity;
        workReport.DefectQuantity = dto.DefectQuantity;
        workReport.Remark = dto.Remark;
        workReport.UpdatedAt = DateTime.UtcNow;

        await _workReportRepository.UpdateAsync(workReport);

        // 更新工单的累计数量
        var workOrder = await _workOrderRepository.GetByIdAsync(workReport.WorkOrderId);
        if (workOrder != null)
        {
            workOrder.CompletedQuantity += goodDiff;
            workOrder.DefectQuantity += defectDiff;

            // 如果累计良品数达到目标产量，自动完工
            if (workOrder.Status == WorkOrderStatus.InProgress && workOrder.CompletedQuantity >= workOrder.TargetQuantity)
            {
                workOrder.Status = WorkOrderStatus.Completed;
                workOrder.CompletedAt = DateTime.UtcNow;
            }

            workOrder.UpdatedAt = DateTime.UtcNow;
            await _workOrderRepository.UpdateAsync(workOrder);
        }

        // 重新查询以获取关联数据
        var workReportWithDetails = await _workReportRepository.GetByIdAsync(id);

        var workReportDto = new WorkReportDto
        {
            Id = workReportWithDetails!.Id,
            WorkOrderId = workReportWithDetails.WorkOrderId,
            OrderNo = workReportWithDetails.WorkOrder.OrderNo,
            ProductName = workReportWithDetails.WorkOrder.Product.ProductName,
            WorkstationId = workReportWithDetails.WorkstationId,
            StationName = workReportWithDetails.Workstation.StationName,
            GoodQuantity = workReportWithDetails.GoodQuantity,
            DefectQuantity = workReportWithDetails.DefectQuantity,
            ReportedBy = workReportWithDetails.ReportedBy,
            ReportedByName = workReportWithDetails.Reporter.RealName,
            ReportedAt = workReportWithDetails.ReportedAt,
            Remark = workReportWithDetails.Remark,
            CreatedAt = workReportWithDetails.CreatedAt,
            UpdatedAt = workReportWithDetails.UpdatedAt
        };

        return ApiResponse<WorkReportDto>.SuccessResult(workReportDto, "报工记录更新成功");
    }

    /// <summary>
    /// 删除报工记录
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteAsync(long id)
    {
        var workReport = await _workReportRepository.GetByIdAsync(id);
        if (workReport == null)
        {
            return ApiResponse<bool>.FailResult("报工记录不存在");
        }

        // 更新工单的累计数量（减去删除的报工数量）
        var workOrder = await _workOrderRepository.GetByIdAsync(workReport.WorkOrderId);
        if (workOrder != null)
        {
            workOrder.CompletedQuantity -= workReport.GoodQuantity;
            workOrder.DefectQuantity -= workReport.DefectQuantity;
            workOrder.UpdatedAt = DateTime.UtcNow;
            await _workOrderRepository.UpdateAsync(workOrder);
        }

        await _workReportRepository.DeleteAsync(workReport);
        return ApiResponse<bool>.SuccessResult(true, "报工记录删除成功");
    }
}
