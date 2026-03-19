// 通用响应类型
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  timestamp: string;
}

// 分页响应类型
export interface PagedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

// 登录相关
export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  realName: string;
  roleName: string;
}

// 产品相关
export interface ProductDto {
  id: number;
  productCode: string;
  productName: string;
  specification?: string;
  status: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateProductDto {
  productCode: string;
  productName: string;
  specification?: string;
}

export interface UpdateProductDto {
  productName: string;
  specification?: string;
  status: number;
}

// 工位相关
export interface WorkstationDto {
  id: number;
  stationCode: string;
  stationName: string;
  workshop?: string;
  status: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateWorkstationDto {
  stationCode: string;
  stationName: string;
  workshop?: string;
}

export interface UpdateWorkstationDto {
  stationName: string;
  workshop?: string;
  status: number;
}

// 工单相关
export interface WorkOrderDto {
  id: number;
  orderNo: string;
  productId: number;
  productCode: string;
  productName: string;
  targetQuantity: number;
  completedQuantity: number;
  defectQuantity: number;
  status: number;
  createdBy: number;
  createdByName: string;
  createdAt: string;
  startedAt?: string;
  completedAt?: string;
  updatedAt: string;
}

export interface CreateWorkOrderDto {
  orderNo: string;
  productId: number;
  targetQuantity: number;
}

export interface UpdateWorkOrderDto {
  targetQuantity: number;
}

// 报工相关
export interface WorkReportDto {
  id: number;
  workOrderId: number;
  orderNo: string;
  productName: string;
  workstationId: number;
  stationName: string;
  goodQuantity: number;
  defectQuantity: number;
  reportedBy: number;
  reportedByName: string;
  reportedAt: string;
  remark?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateWorkReportDto {
  workOrderId: number;
  workstationId: number;
  goodQuantity: number;
  defectQuantity: number;
  remark?: string;
}

export interface UpdateWorkReportDto {
  goodQuantity: number;
  defectQuantity: number;
  remark?: string;
}

// 看板相关
export interface ProductionProgressDto {
  workOrderId: number;
  orderNo: string;
  productName: string;
  targetQuantity: number;
  completedQuantity: number;
  defectQuantity: number;
  completionRate: number;
  qualityRate: number;
  status: number;
  startedAt?: string;
}

export interface WorkstationStatsDto {
  workstationId: number;
  stationName: string;
  workshop?: string;
  totalGoodQuantity: number;
  totalDefectQuantity: number;
  reportCount: number;
  qualityRate: number;
}

export interface ProductStatsDto {
  productId: number;
  productCode: string;
  productName: string;
  totalGoodQuantity: number;
  totalDefectQuantity: number;
  workOrderCount: number;
  qualityRate: number;
}

// 用户相关
export interface UserDto {
  id: number;
  username: string;
  realName: string;
  roleId: number;
  roleName: string;
  status: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateUserDto {
  username: string;
  password: string;
  realName: string;
  roleId: number;
}

export interface UpdateUserDto {
  realName: string;
  roleId: number;
  status: number;
}

export interface ChangePasswordDto {
  oldPassword: string;
  newPassword: string;
}

// 角色相关
export interface RoleDto {
  id: number;
  roleName: string;
  permissions: string;
}

// 设备实时状态
export interface DeviceStatusDto {
  deviceId: string;
  deviceName: string;
  temperature: number;
  speed: number;
  isAlarming: boolean;
  alarmMessage: string;
  timestamp: string;
}
