import request from '../utils/request';
import type {
  ApiResponse,
  PagedResponse,
  WorkOrderDto,
  CreateWorkOrderDto,
  UpdateWorkOrderDto,
} from '../types';

// 获取工单列表
export const getWorkOrders = (params: { page?: number; pageSize?: number; status?: number; keyword?: string }) => {
  return request.get<any, ApiResponse<PagedResponse<WorkOrderDto>>>('/workorders', { params });
};

// 获取工单详情
export const getWorkOrder = (id: number) => {
  return request.get<any, ApiResponse<WorkOrderDto>>(`/workorders/${id}`);
};

// 创建工单
export const createWorkOrder = (data: CreateWorkOrderDto) => {
  return request.post<any, ApiResponse<WorkOrderDto>>('/workorders', data);
};

// 更新工单
export const updateWorkOrder = (id: number, data: UpdateWorkOrderDto) => {
  return request.put<any, ApiResponse<WorkOrderDto>>(`/workorders/${id}`, data);
};

// 取消工单
export const cancelWorkOrder = (id: number) => {
  return request.put<any, ApiResponse<WorkOrderDto>>(`/workorders/${id}/cancel`);
};

// 删除工单
export const deleteWorkOrder = (id: number) => {
  return request.delete<any, ApiResponse<boolean>>(`/workorders/${id}`);
};
