using MiniMES.Domain.Entities;

namespace MiniMES.Application.Interfaces.Repositories;

/// <summary>
/// 产品仓储接口
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// 获取所有产品（分页）
    /// </summary>
    Task<(List<Product> Items, int Total)> GetPagedAsync(int page, int pageSize, string? keyword = null);

    /// <summary>
    /// 根据ID获取产品
    /// </summary>
    Task<Product?> GetByIdAsync(long id);

    /// <summary>
    /// 根据产品编码获取产品
    /// </summary>
    Task<Product?> GetByCodeAsync(string productCode);

    /// <summary>
    /// 创建产品
    /// </summary>
    Task<Product> CreateAsync(Product product);

    /// <summary>
    /// 更新产品
    /// </summary>
    Task UpdateAsync(Product product);

    /// <summary>
    /// 删除产品
    /// </summary>
    Task DeleteAsync(Product product);
}
