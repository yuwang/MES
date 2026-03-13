using MiniMES.Application.DTOs.Product;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Application.Interfaces.Services;
using MiniMES.Domain.Entities;
using MiniMES.Domain.Enums;
using MiniMES.Shared.Common;

namespace MiniMES.Infrastructure.Services;

/// <summary>
/// 产品服务实现
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// 获取产品列表（分页）
    /// </summary>
    public async Task<ApiResponse<PagedResponse<ProductDto>>> GetPagedAsync(int page, int pageSize, string? keyword = null)
    {
        var (items, total) = await _productRepository.GetPagedAsync(page, pageSize, keyword);

        var productDtos = items.Select(p => new ProductDto
        {
            Id = p.Id,
            ProductCode = p.ProductCode,
            ProductName = p.ProductName,
            Specification = p.Specification,
            Status = (int)p.Status,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();

        var pagedResponse = new PagedResponse<ProductDto>
        {
            Items = productDtos,
            Total = total,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponse<PagedResponse<ProductDto>>.SuccessResult(pagedResponse);
    }

    /// <summary>
    /// 根据ID获取产品详情
    /// </summary>
    public async Task<ApiResponse<ProductDto>> GetByIdAsync(long id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return ApiResponse<ProductDto>.FailResult("产品不存在");
        }

        var productDto = new ProductDto
        {
            Id = product.Id,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            Specification = product.Specification,
            Status = (int)product.Status,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return ApiResponse<ProductDto>.SuccessResult(productDto);
    }

    /// <summary>
    /// 创建产品
    /// </summary>
    public async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto dto)
    {
        // 检查产品编码是否已存在
        var existingProduct = await _productRepository.GetByCodeAsync(dto.ProductCode);
        if (existingProduct != null)
        {
            return ApiResponse<ProductDto>.FailResult("产品编码已存在");
        }

        var product = new Product
        {
            ProductCode = dto.ProductCode,
            ProductName = dto.ProductName,
            Specification = dto.Specification,
            Status = EntityStatus.Enabled,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.CreateAsync(product);

        var productDto = new ProductDto
        {
            Id = createdProduct.Id,
            ProductCode = createdProduct.ProductCode,
            ProductName = createdProduct.ProductName,
            Specification = createdProduct.Specification,
            Status = (int)createdProduct.Status,
            CreatedAt = createdProduct.CreatedAt,
            UpdatedAt = createdProduct.UpdatedAt
        };

        return ApiResponse<ProductDto>.SuccessResult(productDto, "产品创建成功");
    }

    /// <summary>
    /// 更新产品
    /// </summary>
    public async Task<ApiResponse<ProductDto>> UpdateAsync(long id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return ApiResponse<ProductDto>.FailResult("产品不存在");
        }

        // 更新产品信息
        product.ProductName = dto.ProductName;
        product.Specification = dto.Specification;
        product.Status = (EntityStatus)dto.Status;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.UpdateAsync(product);

        var productDto = new ProductDto
        {
            Id = product.Id,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            Specification = product.Specification,
            Status = (int)product.Status,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return ApiResponse<ProductDto>.SuccessResult(productDto, "产品更新成功");
    }

    /// <summary>
    /// 删除产品
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteAsync(long id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            return ApiResponse<bool>.FailResult("产品不存在");
        }

        await _productRepository.DeleteAsync(product);
        return ApiResponse<bool>.SuccessResult(true, "产品删除成功");
    }
}
