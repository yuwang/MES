import request from '../utils/request';
import type {
  ApiResponse,
  LoginRequest,
  LoginResponse,
} from '../types';

// 登录
export const login = (data: LoginRequest) => {
  return request.post<any, ApiResponse<LoginResponse>>('/auth/login', data);
};
