using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Application.DTOs.Workstation;
using MiniMES.Application.Interfaces.Services;

namespace MiniMES.API.Controllers;

/// <summary>
/// 工位管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkstationsController : ControllerBase
{
    private readonly IWorkstationService _workstationService;

    public WorkstationsController(IWorkstationService workstationService)
    {
        _workstationService = workstationService;
    }

    /// <summary>
    /// 获取工位列表（分页）
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null)
    {
        var result = await _workstationService.GetPagedAsync(page, pageSize, keyword);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取工位详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _workstationService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// 创建工位
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> Create([FromBody] CreateWorkstationDto dto)
    {
        var result = await _workstationService.CreateAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// 更新工位
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateWorkstationDto dto)
    {
        var result = await _workstationService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// 删除工位
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _workstationService.DeleteAsync(id);
        return Ok(result);
    }
}
