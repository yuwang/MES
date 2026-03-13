using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Application.DTOs.WorkOrder;
using MiniMES.Application.Interfaces.Services;

namespace MiniMES.API.Controllers;

/// <summary>
/// 工单管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkOrdersController : ControllerBase
{
    private readonly IWorkOrderService _workOrderService;

    public WorkOrdersController(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
    }

    /// <summary>
    /// 获取工单列表（分页）
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? status = null,
        [FromQuery] string? keyword = null)
    {
        var result = await _workOrderService.GetPagedAsync(page, pageSize, status, keyword);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取工单详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _workOrderService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// 创建工单
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Planner")]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderDto dto)
    {
        var result = await _workOrderService.CreateAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// 更新工单
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Planner")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateWorkOrderDto dto)
    {
        var result = await _workOrderService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// 取消工单
    /// </summary>
    [HttpPut("{id}/cancel")]
    [Authorize(Roles = "Admin,Planner")]
    public async Task<IActionResult> Cancel(long id)
    {
        var result = await _workOrderService.CancelAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// 删除工单
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _workOrderService.DeleteAsync(id);
        return Ok(result);
    }
}
