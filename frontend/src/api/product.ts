import request from '../utils/request';
import type {
  ApiResponse,
  PagedResponse,
  ProductDto,
  CreateProductDto,
  UpdateProductDto,
} from '../types';

// 获取产品列表
export const getProducts = (params: { page?: number; pageSize?: number; keyword?: string }) => {
  return request.get<any, ApiResponse<PagedResponse<ProductDto>>>('/products', { params });
};

// 获取产品详情
export const getProduct = (id: number) => {
  return request.get<any, ApiResponse<ProductDto>>(`/products/${id}`);
};

// 创建产品
export const createProduct = (data: CreateProductDto) => {
  return request.post<any, ApiResponse<ProductDto>>('/products', data);
};

// 更新产品
export const updateProduct = (id: number, data: UpdateProductDto) => {
  return request.put<any, ApiResponse<ProductDto>>(`/products/${id}`, data);
};

// 删除产品
export const deleteProduct = (id: number) => {
  return request.delete<any, ApiResponse<boolean>>(`/products/${id}`);
};
