using MiniMES.Application.DTOs.Workstation;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Application.Interfaces.Services;
using MiniMES.Domain.Entities;
using MiniMES.Domain.Enums;
using MiniMES.Shared.Common;

namespace MiniMES.Infrastructure.Services;

/// <summary>
/// 工位服务实现
/// </summary>
public class WorkstationService : IWorkstationService
{
    private readonly IWorkstationRepository _workstationRepository;

    public WorkstationService(IWorkstationRepository workstationRepository)
    {
        _workstationRepository = workstationRepository;
    }

    /// <summary>
    /// 获取工位列表（分页）
    /// </summary>
    public async Task<ApiResponse<PagedResponse<WorkstationDto>>> GetPagedAsync(int page, int pageSize, string? keyword = null)
    {
        var (items, total) = await _workstationRepository.GetPagedAsync(page, pageSize, keyword);

        var workstationDtos = items.Select(w => new WorkstationDto
        {
            Id = w.Id,
            StationCode = w.StationCode,
            StationName = w.StationName,
            Workshop = w.Workshop,
            Status = (int)w.Status,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt
        }).ToList();

        var pagedResponse = new PagedResponse<WorkstationDto>
        {
            Items = workstationDtos,
            Total = total,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponse<PagedResponse<WorkstationDto>>.SuccessResult(pagedResponse);
    }

    /// <summary>
    /// 根据ID获取工位详情
    /// </summary>
    public async Task<ApiResponse<WorkstationDto>> GetByIdAsync(long id)
    {
        var workstation = await _workstationRepository.GetByIdAsync(id);
        if (workstation == null)
        {
            return ApiResponse<WorkstationDto>.FailResult("工位不存在");
        }

        var workstationDto = new WorkstationDto
        {
            Id = workstation.Id,
            StationCode = workstation.StationCode,
            StationName = workstation.StationName,
            Workshop = workstation.Workshop,
            Status = (int)workstation.Status,
            CreatedAt = workstation.CreatedAt,
            UpdatedAt = workstation.UpdatedAt
        };

        return ApiResponse<WorkstationDto>.SuccessResult(workstationDto);
    }

    /// <summary>
    /// 创建工位
    /// </summary>
    public async Task<ApiResponse<WorkstationDto>> CreateAsync(CreateWorkstationDto dto)
    {
        // 检查工位编号是否已存在
        var existingWorkstation = await _workstationRepository.GetByCodeAsync(dto.StationCode);
        if (existingWorkstation != null)
        {
            return ApiResponse<WorkstationDto>.FailResult("工位编号已存在");
        }

        var workstation = new Workstation
        {
            StationCode = dto.StationCode,
            StationName = dto.StationName,
            Workshop = dto.Workshop,
            Status = EntityStatus.Enabled,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdWorkstation = await _workstationRepository.CreateAsync(workstation);

        var workstationDto = new WorkstationDto
        {
            Id = createdWorkstation.Id,
            StationCode = createdWorkstation.StationCode,
            StationName = createdWorkstation.StationName,
            Workshop = createdWorkstation.Workshop,
            Status = (int)createdWorkstation.Status,
            CreatedAt = createdWorkstation.CreatedAt,
            UpdatedAt = createdWorkstation.UpdatedAt
        };

        return ApiResponse<WorkstationDto>.SuccessResult(workstationDto, "工位创建成功");
    }

    /// <summary>
    /// 更新工位
    /// </summary>
    public async Task<ApiResponse<WorkstationDto>> UpdateAsync(long id, UpdateWorkstationDto dto)
    {
        var workstation = await _workstationRepository.GetByIdAsync(id);
        if (workstation == null)
        {
            return ApiResponse<WorkstationDto>.FailResult("工位不存在");
        }

        // 更新工位信息
        workstation.StationName = dto.StationName;
        workstation.Workshop = dto.Workshop;
        workstation.Status = (EntityStatus)dto.Status;
        workstation.UpdatedAt = DateTime.UtcNow;

        await _workstationRepository.UpdateAsync(workstation);

        var workstationDto = new WorkstationDto
        {
            Id = workstation.Id,
            StationCode = workstation.StationCode,
            StationName = workstation.StationName,
            Workshop = workstation.Workshop,
            Status = (int)workstation.Status,
            CreatedAt = workstation.CreatedAt,
            UpdatedAt = workstation.UpdatedAt
        };

        return ApiResponse<WorkstationDto>.SuccessResult(workstationDto, "工位更新成功");
    }

    /// <summary>
    /// 删除工位
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteAsync(long id)
    {
        var workstation = await _workstationRepository.GetByIdAsync(id);
        if (workstation == null)
        {
            return ApiResponse<bool>.FailResult("工位不存在");
        }

        await _workstationRepository.DeleteAsync(workstation);
        return ApiResponse<bool>.SuccessResult(true, "工位删除成功");
    }
}
