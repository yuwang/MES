import request from '../utils/request';
import type {
  ApiResponse,
  PagedResponse,
  WorkReportDto,
  CreateWorkReportDto,
  UpdateWorkReportDto,
} from '../types';

// 获取报工记录列表
export const getWorkReports = (params: { page?: number; pageSize?: number; workOrderId?: number }) => {
  return request.get<any, ApiResponse<PagedResponse<WorkReportDto>>>('/workreports', { params });
};

// 获取报工记录详情
export const getWorkReport = (id: number) => {
  return request.get<any, ApiResponse<WorkReportDto>>(`/workreports/${id}`);
};

// 创建报工记录
export const createWorkReport = (data: CreateWorkReportDto) => {
  return request.post<any, ApiResponse<WorkReportDto>>('/workreports', data);
};

// 更新报工记录
export const updateWorkReport = (id: number, data: UpdateWorkReportDto) => {
  return request.put<any, ApiResponse<WorkReportDto>>(`/workreports/${id}`, data);
};

// 删除报工记录
export const deleteWorkReport = (id: number) => {
  return request.delete<any, ApiResponse<boolean>>(`/workreports/${id}`);
};
