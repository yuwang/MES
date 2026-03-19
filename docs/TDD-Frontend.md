# 前端技术设计文档 (TDD)：Mini MES 系统

## 1. 技术栈选型

### 1.1 核心技术

- **框架**：React 19 + TypeScript
- **构建工具**：Vite 7
- **UI 组件库**：Ant Design 6
- **状态管理**：Zustand 5
- **路由**：React Router v7
- **HTTP 客户端**：Axios
- **图表库**：ECharts 6（echarts-for-react）
- **实时通信**：@microsoft/signalr

### 1.2 选型理由

| 技术 | 理由 |
|------|------|
| React 19 | 生态成熟、性能优秀、社区活跃 |
| TypeScript | 类型安全、提升代码质量和可维护性 |
| Vite | 开发体验好、构建速度快 |
| Ant Design 6 | 企业级 UI 组件库、开箱即用 |
| Zustand 5 | 轻量级状态管理、API 简洁 |
| ECharts 6 | 功能强大、适合数据可视化 |
| @microsoft/signalr | 官方 SignalR 客户端，支持自动重连和 JWT 认证 |

## 2. 项目结构设计

```
src/
├── api/                    # API 接口封装
│   ├── auth.ts            # 认证相关接口
│   ├── user.ts            # 用户管理接口
│   ├── product.ts         # 产品管理接口
│   ├── workstation.ts     # 工位管理接口
│   ├── workorder.ts       # 工单管理接口
│   ├── workreport.ts      # 报工管理接口
│   └── dashboard.ts       # 看板数据接口
├── components/            # 通用组件
│   └── Layout.tsx         # 全局布局（Header + Sider + Content）
├── hooks/                 # 自定义 Hooks
│   └── useDeviceMonitor.ts # SignalR 连接封装（设备实时监控）
├── pages/                # 页面组件
│   ├── Login.tsx         # 登录页
│   ├── Dashboard.tsx     # 生产看板
│   ├── DeviceMonitor/    # 设备实时监控（SignalR + ECharts）
│   ├── User/             # 用户管理
│   ├── Product/          # 产品管理
│   ├── Workstation/      # 工位管理
│   ├── WorkOrder/        # 工单管理
│   └── WorkReport/       # 报工管理
├── stores/               # 状态管理
│   └── auth.ts           # 认证状态（Zustand + persist）
├── types/                # TypeScript 类型定义
│   └── index.ts          # 所有 DTO 类型（含 DeviceStatusDto）
├── utils/                # 工具函数
│   └── request.ts        # Axios 封装（自动注入 Bearer token）
└── router/               # 路由配置
    └── index.tsx
```

## 3. 核心功能设计

### 3.1 认证与权限

#### 3.1.1 登录流程

```typescript
// stores/authStore.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
  token: string | null;
  user: User | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  isAuthenticated: () => boolean;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      user: null,
      login: async (username, password) => {
        const response = await authApi.login({ username, password });
        set({ token: response.data.token, user: response.data.user });
      },
      logout: () => {
        set({ token: null, user: null });
      },
      isAuthenticated: () => !!get().token,
    }),
    {
      name: 'auth-storage',
    }
  )
);
```

#### 3.1.2 路由守卫

```typescript
// router/index.tsx
import { Navigate } from 'react-router-dom';
import { useAuthStore } from '@/stores/authStore';

const PrivateRoute = ({ children }: { children: React.ReactNode }) => {
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated());
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" />;
};

const router = createBrowserRouter([
  {
    path: '/login',
    element: <Login />,
  },
  {
    path: '/',
    element: (
      <PrivateRoute>
        <Layout />
      </PrivateRoute>
    ),
    children: [
      { path: 'dashboard', element: <Dashboard /> },
      { path: 'work-orders', element: <WorkOrderList /> },
      { path: 'work-reports', element: <WorkReportList /> },
      // ...
    ],
  },
]);
```

#### 3.1.3 权限控制

```typescript
// hooks/usePermission.ts
import { useAuthStore } from '@/stores/authStore';

export const usePermission = () => {
  const user = useAuthStore((state) => state.user);

  const hasPermission = (permission: string): boolean => {
    if (!user || !user.role) return false;
    return user.role.permissions.includes(permission);
  };

  return { hasPermission };
};

// 使用示例
const WorkOrderPage = () => {
  const { hasPermission } = usePermission();

  return (
    <div>
      {hasPermission('work_order:create') && (
        <Button onClick={handleCreate}>创建工单</Button>
      )}
    </div>
  );
};
```

### 3.2 HTTP 请求封装

```typescript
// utils/request.ts
import axios, { AxiosError, AxiosResponse } from 'axios';
import { message } from 'antd';
import { useAuthStore } from '@/stores/authStore';

const request = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api/v1',
  timeout: 10000,
});

// 请求拦截器
request.interceptors.request.use(
  (config) => {
    const token = useAuthStore.getState().token;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// 响应拦截器
request.interceptors.response.use(
  (response: AxiosResponse<ApiResponse<any>>) => {
    const { success, message: msg, data } = response.data;
    if (!success) {
      message.error(msg || '请求失败');
      return Promise.reject(new Error(msg));
    }
    return data;
  },
  (error: AxiosError<ApiResponse<any>>) => {
    if (error.response?.status === 401) {
      message.error('登录已过期，请重新登录');
      useAuthStore.getState().logout();
      window.location.href = '/login';
    } else {
      message.error(error.response?.data?.message || '网络错误');
    }
    return Promise.reject(error);
  }
);

export default request;
```

### 3.3 API 接口定义

```typescript
// api/workOrder.ts
import request from '@/utils/request';
import { WorkOrder, CreateWorkOrderDto, WorkOrderQuery } from '@/types/models';

export const workOrderApi = {
  // 获取工单列表
  getList: (params: WorkOrderQuery) => {
    return request.get<PagedResponse<WorkOrder>>('/work-orders', { params });
  },

  // 获取工单详情
  getById: (id: number) => {
    return request.get<WorkOrder>(`/work-orders/${id}`);
  },

  // 创建工单
  create: (data: CreateWorkOrderDto) => {
    return request.post<WorkOrder>('/work-orders', data);
  },

  // 取消工单
  cancel: (id: number) => {
    return request.put(`/work-orders/${id}/cancel`);
  },
};
```

### 3.4 类型定义

```typescript
// types/models.ts

// 工单状态枚举
export enum WorkOrderStatus {
  Pending = 0,      // 待产
  InProgress = 1,   // 生产中
  Completed = 2,    // 已完工
  Cancelled = 3,    // 已取消
}

// 工单模型
export interface WorkOrder {
  id: number;
  orderNo: string;
  productId: number;
  productName: string;
  targetQuantity: number;
  completedQuantity: number;
  defectQuantity: number;
  status: WorkOrderStatus;
  createdBy: number;
  createdAt: string;
  startedAt?: string;
  completedAt?: string;
}

// 创建工单 DTO
export interface CreateWorkOrderDto {
  productId: number;
  targetQuantity: number;
}

// 工单查询参数
export interface WorkOrderQuery {
  page?: number;
  pageSize?: number;
  status?: WorkOrderStatus;
  orderNo?: string;
  startDate?: string;
  endDate?: string;
}

// API 响应格式
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  timestamp: string;
}

// 分页响应格式
export interface PagedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
```

## 4. 页面设计

### 4.1 登录页

```typescript
// pages/Login/index.tsx
import { Form, Input, Button, Card } from 'antd';
import { UserOutlined, LockOutlined } from '@ant-design/icons';
import { useAuthStore } from '@/stores/authStore';
import { useNavigate } from 'react-router-dom';

const Login = () => {
  const login = useAuthStore((state) => state.login);
  const navigate = useNavigate();

  const onFinish = async (values: { username: string; password: string }) => {
    try {
      await login(values.username, values.password);
      navigate('/dashboard');
    } catch (error) {
      // 错误已在拦截器中处理
    }
  };

  return (
    <div className="login-container">
      <Card title="Mini MES 系统登录" style={{ width: 400 }}>
        <Form onFinish={onFinish}>
          <Form.Item
            name="username"
            rules={[{ required: true, message: '请输入用户名' }]}
          >
            <Input prefix={<UserOutlined />} placeholder="用户名" />
          </Form.Item>
          <Form.Item
            name="password"
            rules={[{ required: true, message: '请输入密码' }]}
          >
            <Input.Password prefix={<LockOutlined />} placeholder="密码" />
          </Form.Item>
          <Form.Item>
            <Button type="primary" htmlType="submit" block>
              登录
            </Button>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
};

export default Login;
```

### 4.2 工单管理页

```typescript
// pages/WorkOrder/index.tsx
import { useState, useEffect } from 'react';
import { Table, Button, Space, Tag, Modal, Form, Input, Select } from 'antd';
import { workOrderApi } from '@/api/workOrder';
import { productApi } from '@/api/product';
import { WorkOrder, WorkOrderStatus } from '@/types/models';
import { usePermission } from '@/hooks/usePermission';

const WorkOrderList = () => {
  const [dataSource, setDataSource] = useState<WorkOrder[]>([]);
  const [loading, setLoading] = useState(false);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [modalVisible, setModalVisible] = useState(false);
  const [products, setProducts] = useState([]);
  const [form] = Form.useForm();
  const { hasPermission } = usePermission();

  // 加载工单列表
  const loadData = async () => {
    setLoading(true);
    try {
      const response = await workOrderApi.getList({ page, pageSize });
      setDataSource(response.items);
      setTotal(response.total);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, [page, pageSize]);

  // 创建工单
  const handleCreate = async (values: any) => {
    await workOrderApi.create(values);
    setModalVisible(false);
    form.resetFields();
    loadData();
  };

  // 取消工单
  const handleCancel = (id: number) => {
    Modal.confirm({
      title: '确认取消工单？',
      content: '取消后无法恢复',
      onOk: async () => {
        await workOrderApi.cancel(id);
        loadData();
      },
    });
  };

  // 状态标签渲染
  const renderStatus = (status: WorkOrderStatus) => {
    const statusMap = {
      [WorkOrderStatus.Pending]: { text: '待产', color: 'default' },
      [WorkOrderStatus.InProgress]: { text: '生产中', color: 'processing' },
      [WorkOrderStatus.Completed]: { text: '已完工', color: 'success' },
      [WorkOrderStatus.Cancelled]: { text: '已取消', color: 'error' },
    };
    const config = statusMap[status];
    return <Tag color={config.color}>{config.text}</Tag>;
  };

  const columns = [
    { title: '工单号', dataIndex: 'orderNo', key: 'orderNo' },
    { title: '产品名称', dataIndex: 'productName', key: 'productName' },
    { title: '目标产量', dataIndex: 'targetQuantity', key: 'targetQuantity' },
    { title: '完成数量', dataIndex: 'completedQuantity', key: 'completedQuantity' },
    { title: '不良数量', dataIndex: 'defectQuantity', key: 'defectQuantity' },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      render: renderStatus,
    },
    {
      title: '创建时间',
      dataIndex: 'createdAt',
      key: 'createdAt',
      render: (text: string) => new Date(text).toLocaleString(),
    },
    {
      title: '操作',
      key: 'action',
      render: (_: any, record: WorkOrder) => (
        <Space>
          {record.status === WorkOrderStatus.Pending && hasPermission('work_order:cancel') && (
            <Button size="small" danger onClick={() => handleCancel(record.id)}>
              取消
            </Button>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 16 }}>
        {hasPermission('work_order:create') && (
          <Button type="primary" onClick={() => setModalVisible(true)}>
            创建工单
          </Button>
        )}
      </div>

      <Table
        dataSource={dataSource}
        columns={columns}
        loading={loading}
        rowKey="id"
        pagination={{
          current: page,
          pageSize,
          total,
          onChange: (p, ps) => {
            setPage(p);
            setPageSize(ps);
          },
        }}
      />

      <Modal
        title="创建工单"
        open={modalVisible}
        onCancel={() => setModalVisible(false)}
        onOk={() => form.submit()}
      >
        <Form form={form} onFinish={handleCreate} layout="vertical">
          <Form.Item
            name="productId"
            label="产品"
            rules={[{ required: true, message: '请选择产品' }]}
          >
            <Select placeholder="请选择产品">
              {products.map((p: any) => (
                <Select.Option key={p.id} value={p.id}>
                  {p.productName}
                </Select.Option>
              ))}
            </Select>
          </Form.Item>
          <Form.Item
            name="targetQuantity"
            label="目标产量"
            rules={[{ required: true, message: '请输入目标产量' }]}
          >
            <Input type="number" placeholder="请输入目标产量" />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default WorkOrderList;
```

### 4.3 报工提交页

```typescript
// pages/WorkReport/Create.tsx
import { Form, Select, InputNumber, Button, Card } from 'antd';
import { workReportApi } from '@/api/workReport';
import { workOrderApi } from '@/api/workOrder';
import { workstationApi } from '@/api/workstation';
import { useNavigate } from 'react-router-dom';

const CreateWorkReport = () => {
  const [form] = Form.useForm();
  const navigate = useNavigate();
  const [workOrders, setWorkOrders] = useState([]);
  const [workstations, setWorkstations] = useState([]);

  useEffect(() => {
    // 加载待产和生产中的工单
    workOrderApi.getList({ status: [0, 1] }).then((res) => {
      setWorkOrders(res.items);
    });

    // 加载启用的工位
    workstationApi.getList({ status: 1 }).then((res) => {
      setWorkstations(res.items);
    });
  }, []);

  const onFinish = async (values: any) => {
    await workReportApi.create(values);
    navigate('/work-reports');
  };

  return (
    <Card title="提交报工">
      <Form form={form} onFinish={onFinish} layout="vertical">
        <Form.Item
          name="workOrderId"
          label="工单"
          rules={[{ required: true, message: '请选择工单' }]}
        >
          <Select placeholder="请选择工单">
            {workOrders.map((wo: any) => (
              <Select.Option key={wo.id} value={wo.id}>
                {wo.orderNo} - {wo.productName}
              </Select.Option>
            ))}
          </Select>
        </Form.Item>

        <Form.Item
          name="workstationId"
          label="工位"
          rules={[{ required: true, message: '请选择工位' }]}
        >
          <Select placeholder="请选择工位">
            {workstations.map((ws: any) => (
              <Select.Option key={ws.id} value={ws.id}>
                {ws.stationName}
              </Select.Option>
            ))}
          </Select>
        </Form.Item>

        <Form.Item
          name="goodQuantity"
          label="良品数量"
          rules={[{ required: true, message: '请输入良品数量' }]}
        >
          <InputNumber min={0} style={{ width: '100%' }} />
        </Form.Item>

        <Form.Item
          name="defectQuantity"
          label="不良品数量"
          initialValue={0}
        >
          <InputNumber min={0} style={{ width: '100%' }} />
        </Form.Item>

        <Form.Item>
          <Space>
            <Button type="primary" htmlType="submit">
              提交
            </Button>
            <Button onClick={() => navigate('/work-reports')}>
              取消
            </Button>
          </Space>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default CreateWorkReport;
```

### 4.4 生产看板页

```typescript
// pages/Dashboard/index.tsx
import { useEffect, useState } from 'react';
import { Card, Row, Col, Statistic } from 'antd';
import * as echarts from 'echarts';
import { dashboardApi } from '@/api/dashboard';

const Dashboard = () => {
  const [stats, setStats] = useState<any>({});

  useEffect(() => {
    loadData();
    const timer = setInterval(loadData, 5000); // 每 5 秒刷新
    return () => clearInterval(timer);
  }, []);

  const loadData = async () => {
    const [progress, workstationStats, productStats] = await Promise.all([
      dashboardApi.getProgress(),
      dashboardApi.getWorkstationStats(),
      dashboardApi.getProductStats(),
    ]);

    setStats({ progress, workstationStats, productStats });
    renderCharts(workstationStats, productStats);
  };

  const renderCharts = (workstationStats: any[], productStats: any[]) => {
    // 工位产能图表
    const workstationChart = echarts.init(document.getElementById('workstation-chart'));
    workstationChart.setOption({
      title: { text: '工位产能统计' },
      xAxis: { type: 'category', data: workstationStats.map((s) => s.stationName) },
      yAxis: { type: 'value' },
      series: [
        {
          name: '产量',
          type: 'bar',
          data: workstationStats.map((s) => s.totalQuantity),
        },
      ],
    });

    // 产品产量图表
    const productChart = echarts.init(document.getElementById('product-chart'));
    productChart.setOption({
      title: { text: '产品产量统计' },
      series: [
        {
          type: 'pie',
          data: productStats.map((s) => ({
            name: s.productName,
            value: s.totalQuantity,
          })),
        },
      ],
    });
  };

  return (
    <div>
      <Row gutter={16}>
        <Col span={6}>
          <Card>
            <Statistic
              title="整体进度"
              value={stats.progress?.progressPercent || 0}
              suffix="%"
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="当前良率"
              value={stats.progress?.yieldRate || 0}
              suffix="%"
            />
          </Card>
        </Col>
      </Row>

      <Row gutter={16} style={{ marginTop: 16 }}>
        <Col span={12}>
          <Card>
            <div id="workstation-chart" style={{ height: 400 }}></div>
          </Card>
        </Col>
        <Col span={12}>
          <Card>
            <div id="product-chart" style={{ height: 400 }}></div>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default Dashboard;
```

## 5. 样式设计

### 5.1 主题配置

```typescript
// App.tsx
import { ConfigProvider } from 'antd';
import zhCN from 'antd/locale/zh_CN';

const App = () => {
  return (
    <ConfigProvider
      locale={zhCN}
      theme={{
        token: {
          colorPrimary: '#1890ff',
          borderRadius: 4,
        },
      }}
    >
      <RouterProvider router={router} />
    </ConfigProvider>
  );
};
```

### 5.2 响应式设计

- 使用 Ant Design 的栅格系统
- 移动端适配：表格支持横向滚动
- 看板页面支持大屏展示

## 6. 性能优化

### 6.1 代码分割

```typescript
// 路由懒加载
const Dashboard = lazy(() => import('@/pages/Dashboard'));
const WorkOrderList = lazy(() => import('@/pages/WorkOrder'));

const router = createBrowserRouter([
  {
    path: '/',
    element: (
      <Suspense fallback={<Loading />}>
        <Layout />
      </Suspense>
    ),
    children: [
      { path: 'dashboard', element: <Dashboard /> },
      { path: 'work-orders', element: <WorkOrderList /> },
    ],
  },
]);
```

### 6.2 请求优化

- 使用防抖/节流优化搜索输入
- 列表数据使用分页加载
- 看板数据使用轮询刷新（5 秒间隔）

### 6.3 缓存策略

- 基础数据（产品、工位）缓存到 localStorage
- 使用 React Query 管理服务端状态缓存

## 7. 部署配置

### 7.1 环境变量

```bash
# .env.development
VITE_API_BASE_URL=http://localhost:5000/api/v1

# .env.production
VITE_API_BASE_URL=/api/v1
```

### 7.2 Nginx 配置

```nginx
server {
    listen 80;
    server_name mes.example.com;

    root /usr/share/nginx/html;
    index index.html;

    # 前端路由
    location / {
        try_files $uri $uri/ /index.html;
    }

    # API 代理
    location /api/ {
        proxy_pass http://minimes-api:5000;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

## 8. 测试策略

### 8.1 单元测试

- 使用 Vitest + React Testing Library
- 测试组件渲染和交互
- 测试工具函数和 Hooks

### 8.2 E2E 测试

- 使用 Playwright
- 测试关键业务流程（登录、创建工单、提交报工）

## 9. 开发规范

### 9.1 命名规范

- 组件：PascalCase（如 `WorkOrderList`）
- 文件：kebab-case（如 `work-order-list.tsx`）
- 变量/函数：camelCase（如 `handleSubmit`）
- 常量：UPPER_SNAKE_CASE（如 `API_BASE_URL`）

### 9.2 代码规范

- 使用 ESLint + Prettier 统一代码风格
- 使用 TypeScript 严格模式
- 组件拆分：单个组件不超过 300 行
- 使用函数式组件 + Hooks

## 10. 实时通信设计（SignalR）

### 10.1 useDeviceMonitor Hook

封装 SignalR 连接生命周期，页面组件只消费数据，不关心连接细节。

```typescript
// hooks/useDeviceMonitor.ts
export function useDeviceMonitor() {
  // 连接建立：withAutomaticReconnect() 自动处理断线重连
  // JWT 注入：accessTokenFactory 从 localStorage 读取 token
  // 数据存储：Map<deviceId, { latest, history[] }>
  //           每台设备保留最近 60 条（对应 60 秒滚动窗口）
  return { devices, connected, error };
}
```

### 10.2 Vite 开发代理

开发环境需同时代理 REST API 和 SignalR WebSocket：

```typescript
// vite.config.ts
proxy: {
  '/api': { target: 'http://localhost:5000', changeOrigin: true },
  '/hubs': { target: 'http://localhost:5000', changeOrigin: true, ws: true },
}
```

`ws: true` 是关键，缺少此配置 SignalR 协商请求会返回 404。

### 10.3 DeviceMonitor 页面结构

```
DeviceMonitor
├── 连接状态 Badge（绿/红）
└── Row（每台设备一个 Col）
    └── Card（设备名 + 报警/正常 Tag）
        ├── Alert 区域（固定高度，visibility 控制显隐，不影响下方布局）
        ├── Statistic Row（当前温度 + 转速，超阈值变红）
        └── ReactECharts（双 Y 轴折线图，animation: false，60 秒滚动窗口）
```

### 10.4 ECharts 配置要点

- `animation: false`：每秒刷新场景下关闭动画，避免视觉抖动
- 双 Y 轴：温度（50-110°C）和转速（800-3200 RPM）量纲差异大，各用独立 Y 轴
- `markLine`：在图上绘制报警阈值参考线（温度 90°C / 转速 2800 RPM）
- `notMerge`：每次数据更新完全替换 option，避免历史数据残留
