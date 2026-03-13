using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniMES.Application.DTOs.Product;
using MiniMES.Application.Interfaces.Services;

namespace MiniMES.API.Controllers;

/// <summary>
/// 产品管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// 获取产品列表（分页）
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null)
    {
        var result = await _productService.GetPagedAsync(page, pageSize, keyword);
        return Ok(result);
    }

    /// <summary>
    /// 根据ID获取产品详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _productService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// 创建产品
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var result = await _productService.CreateAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// 更新产品
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Technician")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateProductDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto);
        return Ok(result);
    }

    /// <summary>
    /// 删除产品
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _productService.DeleteAsync(id);
        return Ok(result);
    }
}
