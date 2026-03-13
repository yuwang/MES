using MiniMES.Application.DTOs.Product;
using MiniMES.Shared.Common;

namespace MiniMES.Application.Interfaces.Services;

/// <summary>
/// 产品服务接口
/// </summary>
public interface IProductService
{
    /// <summary>
    /// 获取产品列表（分页）
    /// </summary>
    Task<ApiResponse<PagedResponse<ProductDto>>> GetPagedAsync(int page, int pageSize, string? keyword = null);

    /// <summary>
    /// 根据ID获取产品详情
    /// </summary>
    Task<ApiResponse<ProductDto>> GetByIdAsync(long id);

    /// <summary>
    /// 创建产品
    /// </summary>
    Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto);

    /// <summary>
    /// 更新产品
    /// </summary>
    Task<ApiResponse<ProductDto>> UpdateAsync(long id, UpdateProductDto dto);

    /// <summary>
    /// 删除产品
    /// </summary>
    Task<ApiResponse<bool>> DeleteAsync(long id);
}
