import request from '../utils/request';
import type {
  ApiResponse,
  ProductionProgressDto,
  WorkstationStatsDto,
  ProductStatsDto,
} from '../types';

// 获取生产进度监控
export const getProductionProgress = () => {
  return request.get<any, ApiResponse<ProductionProgressDto[]>>('/dashboard/progress');
};

// 获取工位产能统计
export const getWorkstationStats = (params?: { startDate?: string; endDate?: string }) => {
  return request.get<any, ApiResponse<WorkstationStatsDto[]>>('/dashboard/workstation-stats', { params });
};

// 获取产品产量统计
export const getProductStats = (params?: { startDate?: string; endDate?: string }) => {
  return request.get<any, ApiResponse<ProductStatsDto[]>>('/dashboard/product-stats', { params });
};
