using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Application.DTOs.WorkReport;
using MiniMES.Application.Interfaces.Services;

namespace MiniMES.API.Controllers;

/// <summary>
/// 报工管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkReportsController : ControllerBase
{
    private readonly IWorkReportService _workReportService;

    public WorkReportsController(IWorkReportService workReportService)
    {
        _workReportService = workReportService;
    }

    /// <summary>
    /// 获取报工记录列表（分页）
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] long? workOrderId = null)
    {
        var result = await _workReportService.GetPagedAsync(page, pageSize, workOrderId);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取报工记录详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _workReportService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// 创建报工记录
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> Create([FromBody] CreateWorkReportDto dto)
    {
        var result = await _workReportService.CreateAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// 更新报工记录
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Operator")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateWorkReportDto dto)
    {
        var result = await _workReportService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// 删除报工记录
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _workReportService.DeleteAsync(id);
        return Ok(result);
    }
}
