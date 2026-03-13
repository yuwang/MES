import request from '../utils/request';
import type {
  ApiResponse,
  PagedResponse,
  WorkstationDto,
  CreateWorkstationDto,
  UpdateWorkstationDto,
} from '../types';

// 获取工位列表
export const getWorkstations = (params: { page?: number; pageSize?: number; keyword?: string }) => {
  return request.get<any, ApiResponse<PagedResponse<WorkstationDto>>>('/workstations', { params });
};

// 获取工位详情
export const getWorkstation = (id: number) => {
  return request.get<any, ApiResponse<WorkstationDto>>(`/workstations/${id}`);
};

// 创建工位
export const createWorkstation = (data: CreateWorkstationDto) => {
  return request.post<any, ApiResponse<WorkstationDto>>('/workstations', data);
};

// 更新工位
export const updateWorkstation = (id: number, data: UpdateWorkstationDto) => {
  return request.put<any, ApiResponse<WorkstationDto>>(`/workstations/${id}`, data);
};

// 删除工位
export const deleteWorkstation = (id: number) => {
  return request.delete<any, ApiResponse<boolean>>(`/workstations/${id}`);
};
