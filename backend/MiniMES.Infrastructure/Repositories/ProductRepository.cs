using Microsoft.EntityFrameworkCore;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Data;

namespace MiniMES.Infrastructure.Repositories;

/// <summary>
/// 产品仓储实现
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly MiniMesDbContext _context;

    public ProductRepository(MiniMesDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取所有产品（分页）
    /// </summary>
    public async Task<(List<Product> Items, int Total)> GetPagedAsync(int page, int pageSize, string? keyword = null)
    {
        var query = _context.Products.AsNoTracking();

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p => p.ProductCode.Contains(keyword) || p.ProductName.Contains(keyword));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    /// <summary>
    /// 根据ID获取产品
    /// </summary>
    public async Task<Product?> GetByIdAsync(long id)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// 根据产品编码获取产品
    /// </summary>
    public async Task<Product?> GetByCodeAsync(string productCode)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductCode == productCode);
    }

    /// <summary>
    /// 创建产品
    /// </summary>
    public async Task<Product> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    /// <summary>
    /// 更新产品
    /// </summary>
    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 删除产品
    /// </summary>
    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}
