using MiniMES.Application.DTOs.WorkOrder;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Application.Interfaces.Services;
using MiniMES.Domain.Entities;
using MiniMES.Domain.Enums;
using MiniMES.Shared.Common;

namespace MiniMES.Infrastructure.Services;

/// <summary>
/// 工单服务实现
/// </summary>
public class WorkOrderService : IWorkOrderService
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;

    public WorkOrderService(
        IWorkOrderRepository workOrderRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService)
    {
        _workOrderRepository = workOrderRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// 获取工单列表（分页）
    /// </summary>
    public async Task<ApiResponse<PagedResponse<WorkOrderDto>>> GetPagedAsync(int page, int pageSize, int? status = null, string? keyword = null)
    {
        var (items, total) = await _workOrderRepository.GetPagedAsync(page, pageSize, status, keyword);

        var workOrderDtos = items.Select(w => new WorkOrderDto
        {
            Id = w.Id,
            OrderNo = w.OrderNo,
            ProductId = w.ProductId,
            ProductCode = w.Product.ProductCode,
            ProductName = w.Product.ProductName,
            TargetQuantity = w.TargetQuantity,
            CompletedQuantity = w.CompletedQuantity,
            DefectQuantity = w.DefectQuantity,
            Status = (int)w.Status,
            CreatedBy = w.CreatedBy,
            CreatedByName = w.Creator.RealName,
            CreatedAt = w.CreatedAt,
            StartedAt = w.StartedAt,
            CompletedAt = w.CompletedAt,
            UpdatedAt = w.UpdatedAt
        }).ToList();

        var pagedResponse = new PagedResponse<WorkOrderDto>
        {
            Items = workOrderDtos,
            Total = total,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponse<PagedResponse<WorkOrderDto>>.SuccessResult(pagedResponse);
    }

    /// <summary>
    /// 根据ID获取工单详情
    /// </summary>
    public async Task<ApiResponse<WorkOrderDto>> GetByIdAsync(long id)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id);
        if (workOrder == null)
        {
            return ApiResponse<WorkOrderDto>.FailResult("工单不存在");
        }

        var workOrderDto = new WorkOrderDto
        {
            Id = workOrder.Id,
            OrderNo = workOrder.OrderNo,
            ProductId = workOrder.ProductId,
            ProductCode = workOrder.Product.ProductCode,
            ProductName = workOrder.Product.ProductName,
            TargetQuantity = workOrder.TargetQuantity,
            CompletedQuantity = workOrder.CompletedQuantity,
            DefectQuantity = workOrder.DefectQuantity,
            Status = (int)workOrder.Status,
            CreatedBy = workOrder.CreatedBy,
            CreatedByName = workOrder.Creator.RealName,
            CreatedAt = workOrder.CreatedAt,
            StartedAt = workOrder.StartedAt,
            CompletedAt = workOrder.CompletedAt,
            UpdatedAt = workOrder.UpdatedAt
        };

        return ApiResponse<WorkOrderDto>.SuccessResult(workOrderDto);
    }

    /// <summary>
    /// 创建工单
    /// </summary>
    public async Task<ApiResponse<WorkOrderDto>> CreateAsync(CreateWorkOrderDto dto)
    {
        // 检查工单号是否已存在
        var existingWorkOrder = await _workOrderRepository.GetByOrderNoAsync(dto.OrderNo);
        if (existingWorkOrder != null)
        {
            return ApiResponse<WorkOrderDto>.FailResult("工单号已存在");
        }

        // 检查产品是否存在
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
        {
            return ApiResponse<WorkOrderDto>.FailResult("产品不存在");
        }

        // 检查产品是否启用
        if (product.Status == EntityStatus.Disabled)
        {
            return ApiResponse<WorkOrderDto>.FailResult("产品已停用，无法创建工单");
        }

        var currentUserId = _currentUserService.GetUserId();
        var workOrder = new WorkOrder
        {
            OrderNo = dto.OrderNo,
            ProductId = dto.ProductId,
            TargetQuantity = dto.TargetQuantity,
            CompletedQuantity = 0,
            DefectQuantity = 0,
            Status = WorkOrderStatus.Pending,
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdWorkOrder = await _workOrderRepository.CreateAsync(workOrder);

        // 重新查询以获取关联数据
        var workOrderWithDetails = await _workOrderRepository.GetByIdAsync(createdWorkOrder.Id);

        var workOrderDto = new WorkOrderDto
        {
            Id = workOrderWithDetails!.Id,
            OrderNo = workOrderWithDetails.OrderNo,
            ProductId = workOrderWithDetails.ProductId,
            ProductCode = workOrderWithDetails.Product.ProductCode,
            ProductName = workOrderWithDetails.Product.ProductName,
            TargetQuantity = workOrderWithDetails.TargetQuantity,
            CompletedQuantity = workOrderWithDetails.CompletedQuantity,
            DefectQuantity = workOrderWithDetails.DefectQuantity,
            Status = (int)workOrderWithDetails.Status,
            CreatedBy = workOrderWithDetails.CreatedBy,
            CreatedByName = workOrderWithDetails.Creator.RealName,
            CreatedAt = workOrderWithDetails.CreatedAt,
            StartedAt = workOrderWithDetails.StartedAt,
            CompletedAt = workOrderWithDetails.CompletedAt,
            UpdatedAt = workOrderWithDetails.UpdatedAt
        };

        return ApiResponse<WorkOrderDto>.SuccessResult(workOrderDto, "工单创建成功");
    }

    /// <summary>
    /// 更新工单
    /// </summary>
    public async Task<ApiResponse<WorkOrderDto>> UpdateAsync(long id, UpdateWorkOrderDto dto)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id);
        if (workOrder == null)
        {
            return ApiResponse<WorkOrderDto>.FailResult("工单不存在");
        }

        // 只有待产状态的工单才能修改目标产量
        if (workOrder.Status != WorkOrderStatus.Pending)
        {
            return ApiResponse<WorkOrderDto>.FailResult("只有待产状态的工单才能修改");
        }

        workOrder.TargetQuantity = dto.TargetQuantity;
        workOrder.UpdatedAt = DateTime.UtcNow;

        await _workOrderRepository.UpdateAsync(workOrder);

        // 重新查询以获取关联数据
        var workOrderWithDetails = await _workOrderRepository.GetByIdAsync(id);

        var workOrderDto = new WorkOrderDto
        {
            Id = workOrderWithDetails!.Id,
            OrderNo = workOrderWithDetails.OrderNo,
            ProductId = workOrderWithDetails.ProductId,
            ProductCode = workOrderWithDetails.Product.ProductCode,
            ProductName = workOrderWithDetails.Product.ProductName,
            TargetQuantity = workOrderWithDetails.TargetQuantity,
            CompletedQuantity = workOrderWithDetails.CompletedQuantity,
            DefectQuantity = workOrderWithDetails.DefectQuantity,
            Status = (int)workOrderWithDetails.Status,
            CreatedBy = workOrderWithDetails.CreatedBy,
            CreatedByName = workOrderWithDetails.Creator.RealName,
            CreatedAt = workOrderWithDetails.CreatedAt,
            StartedAt = workOrderWithDetails.StartedAt,
            CompletedAt = workOrderWithDetails.CompletedAt,
            UpdatedAt = workOrderWithDetails.UpdatedAt
        };

        return ApiResponse<WorkOrderDto>.SuccessResult(workOrderDto, "工单更新成功");
    }

    /// <summary>
    /// 取消工单
    /// </summary>
    public async Task<ApiResponse<WorkOrderDto>> CancelAsync(long id)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id);
        if (workOrder == null)
        {
            return ApiResponse<WorkOrderDto>.FailResult("工单不存在");
        }

        // 只有待产和生产中的工单才能取消
        if (workOrder.Status != WorkOrderStatus.Pending && workOrder.Status != WorkOrderStatus.InProgress)
        {
            return ApiResponse<WorkOrderDto>.FailResult("只有待产或生产中的工单才能取消");
        }

        workOrder.Status = WorkOrderStatus.Cancelled;
        workOrder.UpdatedAt = DateTime.UtcNow;

        await _workOrderRepository.UpdateAsync(workOrder);

        // 重新查询以获取关联数据
        var workOrderWithDetails = await _workOrderRepository.GetByIdAsync(id);

        var workOrderDto = new WorkOrderDto
        {
            Id = workOrderWithDetails!.Id,
            OrderNo = workOrderWithDetails.OrderNo,
            ProductId = workOrderWithDetails.ProductId,
            ProductCode = workOrderWithDetails.Product.ProductCode,
            ProductName = workOrderWithDetails.Product.ProductName,
            TargetQuantity = workOrderWithDetails.TargetQuantity,
            CompletedQuantity = workOrderWithDetails.CompletedQuantity,
            DefectQuantity = workOrderWithDetails.DefectQuantity,
            Status = (int)workOrderWithDetails.Status,
            CreatedBy = workOrderWithDetails.CreatedBy,
            CreatedByName = workOrderWithDetails.Creator.RealName,
            CreatedAt = workOrderWithDetails.CreatedAt,
            StartedAt = workOrderWithDetails.StartedAt,
            CompletedAt = workOrderWithDetails.CompletedAt,
            UpdatedAt = workOrderWithDetails.UpdatedAt
        };

        return ApiResponse<WorkOrderDto>.SuccessResult(workOrderDto, "工单已取消");
    }

    /// <summary>
    /// 删除工单
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteAsync(long id)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id);
        if (workOrder == null)
        {
            return ApiResponse<bool>.FailResult("工单不存在");
        }

        // 只有待产状态的工单才能删除
        if (workOrder.Status != WorkOrderStatus.Pending)
        {
            return ApiResponse<bool>.FailResult("只有待产状态的工单才能删除");
        }

        await _workOrderRepository.DeleteAsync(workOrder);
        return ApiResponse<bool>.SuccessResult(true, "工单删除成功");
    }
}
