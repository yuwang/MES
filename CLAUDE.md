# MiniMES — CLAUDE.md

## 项目概述

MiniMES 是一个轻量级制造执行系统（MES），用于管理生产工单、工位、报工和设备实时监控。

## 技术栈

| 层 | 技术 |
|----|------|
| 后端框架 | ASP.NET Core 8.0 |
| ORM | EF Core 8.0 + Pomelo MySQL |
| 实时推送 | ASP.NET Core SignalR |
| 认证 | JWT Bearer |
| 前端框架 | React 19 + TypeScript + Vite |
| UI 组件库 | Ant Design 6 |
| 状态管理 | Zustand 5（仅 auth store） |
| 路由 | React Router 7 |
| HTTP 客户端 | Axios（封装在 `src/utils/request.ts`） |
| 图表 | ECharts 6（`echarts-for-react`） |
| SignalR 客户端 | `@microsoft/signalr` |

## 目录结构

```
MiniMES/
├── backend/
│   ├── MiniMES.API/              # 表现层：Controllers、Hubs、Middlewares、BackgroundServices
│   ├── MiniMES.Application/      # 应用层：接口定义（IService、IRepository）、DTOs
│   ├── MiniMES.Domain/           # 领域层：实体（BaseEntity 子类）、枚举
│   ├── MiniMES.Infrastructure/   # 基础设施层：EF Core、Repository 实现、Service 实现
│   └── MiniMES.Shared/           # 共享：ApiResponse<T>、PagedResponse<T>
└── frontend/
    └── src/
        ├── api/          # 按模块拆分的 API 调用函数
        ├── components/   # 全局组件（Layout）
        ├── hooks/        # 自定义 hooks（useDeviceMonitor）
        ├── pages/        # 页面组件（按模块分目录）
        ├── router/       # 路由配置
        ├── stores/       # Zustand stores
        ├── types/        # 全局 TypeScript 类型
        └── utils/        # 工具函数（request.ts Axios 封装）
```

## 架构规范

### 后端

- **分层依赖**：API → Application + Infrastructure，Infrastructure → Application → Domain，Shared 被所有层引用
- **Controller 模式**：继承 `ControllerBase`，标注 `[ApiController]`、`[Route("api/[controller]")]`、`[Authorize]`，返回 `ApiResponse<T>`
- **DI 注册**：所有 Service/Repository 在 `MiniMES.Infrastructure/DependencyInjection.cs` 中注册为 Scoped
- **BackgroundService**：放在 `MiniMES.API/BackgroundServices/`（因为直接依赖 Hub）
- **SignalR Hub**：放在 `MiniMES.API/Hubs/`，标注 `[Authorize]`
- **CORS**：REST API 使用 `AllowAll` 策略；SignalR Hub 使用 `AllowFrontend` 策略（从配置读取允许来源，支持 `AllowCredentials`）

### 前端

- **API 调用**：统一通过 `src/utils/request.ts` 的 Axios 实例，自动注入 Bearer token，统一处理 401/403/500
- **类型定义**：所有 DTO 类型集中在 `src/types/index.ts`
- **列表页模式**：`useState(loading/dataSource/total/page)` + `useEffect` 触发 `loadData()` + Table + Modal(Form)
- **实时数据**：通过 `src/hooks/useDeviceMonitor.ts` 封装 SignalR 连接生命周期

## 运行方式

```bash
# 后端（需要 MySQL，先配置 appsettings.Development.json）
cd backend && dotnet run --project MiniMES.API

# 前端
cd frontend && npm run dev
```

## CORS 配置

- 开发环境：`appsettings.json` 中 `Cors.AllowedOrigins: ["http://localhost:5173"]`
- 生产环境（nginx 同机部署）：`appsettings.Production.json` 中 `Cors.AllowedOrigins: ["http://localhost", "http://127.0.0.1"]`
- 若有域名，改为实际域名，如 `["http://mes.example.com"]`

## SignalR 事件

| 事件名 | 方向 | 数据 |
|--------|------|------|
| `ReceiveDeviceStatus` | 服务端 → 客户端 | `DeviceStatusDto[]` |
| `SubscribeDevice(deviceId)` | 客户端 → 服务端 | 加入设备专属 Group |
| `UnsubscribeDevice(deviceId)` | 客户端 → 服务端 | 离开设备专属 Group |
