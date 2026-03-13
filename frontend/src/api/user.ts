import request from '../utils/request';
import type {
  ApiResponse,
  PagedResponse,
  UserDto,
  CreateUserDto,
  UpdateUserDto,
  ChangePasswordDto,
} from '../types';

// 获取用户列表
export const getUsers = (params: { page?: number; pageSize?: number; keyword?: string }) => {
  return request.get<any, ApiResponse<PagedResponse<UserDto>>>('/users', { params });
};

// 获取用户详情
export const getUser = (id: number) => {
  return request.get<any, ApiResponse<UserDto>>(`/users/${id}`);
};

// 创建用户
export const createUser = (data: CreateUserDto) => {
  return request.post<any, ApiResponse<UserDto>>('/users', data);
};

// 更新用户
export const updateUser = (id: number, data: UpdateUserDto) => {
  return request.put<any, ApiResponse<UserDto>>(`/users/${id}`, data);
};

// 删除用户
export const deleteUser = (id: number) => {
  return request.delete<any, ApiResponse<boolean>>(`/users/${id}`);
};

// 修改密码
export const changePassword = (id: number, data: ChangePasswordDto) => {
  return request.put<any, ApiResponse<boolean>>(`/users/${id}/change-password`, data);
};

// 重置密码
export const resetPassword = (id: number, newPassword: string) => {
  return request.put<any, ApiResponse<boolean>>(`/users/${id}/reset-password`, newPassword);
};
