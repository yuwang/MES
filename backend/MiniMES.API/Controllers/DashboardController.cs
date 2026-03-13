using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Application.Interfaces.Services;

namespace MiniMES.API.Controllers;

/// <summary>
/// 生产看板控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// 获取生产进度监控数据
    /// </summary>
    [HttpGet("progress")]
    public async Task<IActionResult> GetProductionProgress()
    {
        var result = await _dashboardService.GetProductionProgressAsync();
        return Ok(result);
    }

    /// <summary>
    /// 获取工位产能统计
    /// </summary>
    [HttpGet("workstation-stats")]
    public async Task<IActionResult> GetWorkstationStats([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var result = await _dashboardService.GetWorkstationStatsAsync(startDate, endDate);
        return Ok(result);
    }

    /// <summary>
    /// 获取产品产量统计
    /// </summary>
    [HttpGet("product-stats")]
    public async Task<IActionResult> GetProductStats([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var result = await _dashboardService.GetProductStatsAsync(startDate, endDate);
        return Ok(result);
    }
}
