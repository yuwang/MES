# Mini MES 系统

一个轻量级的制造执行系统（MES），用于解决车间现场的核心生产管理问题。

## 项目结构

```
MiniMES/
├── docs/                    # 文档目录
│   ├── PRD.md              # 产品需求文档
│   ├── TDD-Backend.md      # 后端技术设计文档
│   └── TDD-Frontend.md     # 前端技术设计文档
├── backend/                # 后端项目
│   ├── MiniMES.API/        # Web API 层
│   ├── MiniMES.Application/ # 应用服务层
│   ├── MiniMES.Domain/     # 领域模型层
│   ├── MiniMES.Infrastructure/ # 基础设施层
│   ├── MiniMES.Shared/     # 共享层
│   └── MiniMES.slnx        # 解决方案文件
├── frontend/               # 前端项目
│   ├── src/                # 源代码
│   ├── package.json        # 依赖配置
│   └── vite.config.ts      # Vite 配置
└── README.md              # 项目说明
```

## 技术栈

### 后端

- .NET 8 Web API
- MySQL 8.0
- Entity Framework Core
- JWT 认证
- Swagger/OpenAPI

### 前端

- React 18 + TypeScript
- Vite 5
- Ant Design 5
- Zustand (状态管理)
- React Router v6
- Axios
- ECharts

## 配置说明

### 首次使用配置

克隆项目后，需要配置数据库连接和其他敏感信息：

**1. 复制配置模板**

```bash
cd backend/MiniMES.API
cp appsettings.Development.json.example appsettings.Development.json
```

**2. 编辑配置文件**

打开 `appsettings.Development.json`，填写你的数据库连接信息：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_MYSQL_HOST;Port=3306;Database=minimes;User=YOUR_MYSQL_USER;Password=YOUR_MYSQL_PASSWORD;"
  },
  "Jwt": {
    "Key": "CHANGE_THIS_TO_A_SECURE_SECRET_KEY_AT_LEAST_32_CHARACTERS"
  }
}
```

**3. 安全注意事项**

⚠️ **重要**：

- `appsettings.Development.json` 已添加到 `.gitignore`，不会被提交到 Git
- 请勿将真实的数据库密码、JWT 密钥等敏感信息提交到代码仓库
- **JWT 密钥要求**：至少 32 个字符，建议使用随机生成的强密钥
- 生产环境建议使用环境变量或密钥管理服务

**生成安全的 JWT 密钥示例**：

```bash
# Linux/macOS
openssl rand -base64 32

# 或使用在线工具生成 32+ 字符的随机字符串
```

## 快速开始

### 前置要求

- .NET 8 SDK
- Node.js 20+
- MySQL 8.0

### 本地开发

#### 1. 准备数据库

确保 MySQL 8.0 已安装并运行。创建数据库：

```bash
mysql -u root -p
CREATE DATABASE minimes CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
EXIT;
```

或使用远程 MySQL 服务器（如阿里云）。

#### 2. 启动后端

```bash
cd backend/MiniMES.API
dotnet restore
dotnet ef database update  # 创建数据库表
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

后端 API 地址：http://localhost:5000
Swagger 文档：http://localhost:5000/swagger

**注意**：必须设置 `ASPNETCORE_ENVIRONMENT=Development` 才能访问 Swagger 文档。

#### 3. 启动前端

```bash
cd frontend
npm install
npm run dev
```

前端地址：http://localhost:3000

**默认登录账号**：

- 用户名：`admin`
- 密码：`admin123`

## 初始化测试数据

系统启动后会自动创建默认角色和管理员账号。如需更多测试数据，可以执行初始化脚本：

```bash
mysql -h your_host -u your_user -p minimes < docs/init-data.sql
```

该脚本会创建：

- 5个测试用户（不同角色）
- 5个产品数据
- 5个工位数据
- 5个工单数据（不同状态）
- 若干报工记录

## 生产部署

📦 **详细的部署指南请查看：[deploy/README.md](deploy/README.md)**

快速部署步骤：

1. 本地构建项目（后端 + 前端）
2. 配置部署脚本（复制 `deploy/deploy.sh.example` 为 `deploy.sh`）
3. 在服务器上安装依赖（.NET Runtime、Nginx、MySQL）
4. 执行部署脚本自动上传和启动

支持的部署方式：

- ✅ 自动化部署脚本（推荐）
- ✅ 手动 SCP 上传

### 环境变量说明

- `ASPNETCORE_ENVIRONMENT`: 环境名称（Development/Production）
- `DOTNET_ENVIRONMENT`: 备用环境变量
- 默认为 Production

## 核心功能

- **用户权限管理**：用户登录、角色权限控制
- **基础数据管理**：产品管理、工位管理
- **生产工单管理**：工单下发、状态流转、工单查询
- **车间报工执行**：现场报工提交、报工明细查询
- **生产看板数据**：实时进度监控、良率统计、产能分析

## 开发指南

### 后端开发

项目采用分层架构：

- **API 层**：控制器、中间件
- **Application 层**：业务逻辑、服务
- **Domain 层**：实体模型、枚举
- **Infrastructure 层**：数据访问、EF Core 配置
- **Shared 层**：DTO、响应格式、工具类

### 前端开发

推荐目录结构：

```
src/
├── api/          # API 接口
├── components/   # 通用组件
├── pages/        # 页面组件
├── stores/       # 状态管理
├── types/        # 类型定义
├── utils/        # 工具函数
└── router/       # 路由配置
```

## 数据库迁移

```bash
# 添加迁移
cd backend/MiniMES.API
dotnet ef migrations add InitialCreate

# 更新数据库
dotnet ef database update

# 回滚迁移
dotnet ef database update PreviousMigrationName
```

## 测试

### 后端测试

```bash
cd backend
dotnet test
```

### 前端测试

```bash
cd frontend
npm run test
```

## 未来规划

### 🚀 功能扩展

- [ ] **设备对接**
  - PLC 集成（西门子、三菱、欧姆龙）
  - 扫码枪/RFID 自动采集
  - 电子看板实时推送
  - 支持 Modbus TCP/RTU、OPC UA、MQTT 协议

- [ ] **高级报表**
  - 生产日报/周报/月报
  - 设备 OEE 分析
  - 质量追溯报表
  - 自定义报表设计器

- [ ] **移动端支持**
  - 移动端报工 App（React Native）
  - 微信小程序报工
  - 扫码快速报工

- [ ] **质量管理**
  - 首检/巡检/终检流程
  - 不良品追溯
  - SPC 统计过程控制
  - 质量异常预警

- [ ] **物料管理**
  - 物料需求计划（MRP）
  - 库存预警
  - 物料批次追溯
  - 条码管理

- [ ] **系统增强**
  - 多工厂/多车间支持
  - 数据权限隔离
  - 操作日志审计
  - 系统性能监控

## 许可证

MIT License
