# MiniMES 部署指南

本指南将帮助你将 MiniMES 系统部署到 Linux 服务器上，使用 Nginx 作为反向代理。

## 📋 前置要求

### 本地环境

- .NET 8 SDK
- Node.js 20+
- rsync（macOS/Linux 自带）

### 服务器环境

- Ubuntu 20.04+ / CentOS 8+
- .NET 8 Runtime
- Nginx
- MySQL 8.0
- 至少 2GB 内存

---

## 🚀 快速部署

### 1. 本地构建

```bash
# 后端
cd backend/MiniMES.API
dotnet publish -c Release -o ./publish

# 前端
cd frontend
npm run build
```

### 2. 配置部署脚本

```bash
cp deploy/deploy.sh.example deploy/deploy.sh
vim deploy/deploy.sh
```

修改服务器配置：

```bash
SERVER_USER="your-username"
SERVER_HOST="your-server-ip"
```

### 3. 服务器初始化（首次部署）

SSH 登录服务器，执行以下步骤：

#### 3.1 安装依赖（如需要）

```bash
# 安装 .NET 8 Runtime
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0 --runtime aspnetcore --install-dir /usr/local/dotnet
sudo ln -s /usr/local/dotnet/dotnet /usr/bin/dotnet

# 安装 rsync
sudo yum install -y rsync  # CentOS/RHEL
# 或
sudo apt install -y rsync  # Ubuntu/Debian
```

#### 3.2 创建目录

```bash
sudo mkdir -p /opt/apps/minimes /var/www/minimes

# 设置权限（根据系统选择）
sudo chown -R nginx:nginx /opt/apps/minimes /var/www/minimes      # CentOS/RHEL
# 或
sudo chown -R www-data:www-data /opt/apps/minimes /var/www/minimes  # Ubuntu/Debian
```

#### 3.3 配置 Nginx

编辑 `/etc/nginx/nginx.conf`，在 `http {}` 块内添加：

```nginx
server {
    listen 3001;  # 或其他端口
    server_name your-server-ip;

    location / {
        root /var/www/minimes;
        try_files $uri $uri/ /index.html;
    }

    location /api/ {
        proxy_pass http://localhost:5000/api/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    }

    access_log /var/log/nginx/minimes_access.log;
    error_log /var/log/nginx/minimes_error.log;
}
```

测试并重载：

```bash
sudo nginx -t
sudo systemctl reload nginx
```

#### 3.4 配置生产环境

⚠️ 安全密钥可手动指定，或生成：`openssl rand -base64 32`

创建 `/opt/apps/minimes/appsettings.Production.json`：

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=minimes;User=root;Password=YOUR_MYSQL_PASSWORD;"
  },
  "Jwt": {
    "Key": "YOUR_PRODUCTION_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "MiniMES",
    "Audience": "MiniMES",
    "ExpirationHours": 8
  }
}
```

#### 3.5 配置 systemd 服务

创建 `/etc/systemd/system/minimes.service`：

```ini
[Unit]
Description=MiniMES API Service
After=network.target

[Service]
WorkingDirectory=/opt/apps/minimes
ExecStart=/usr/bin/dotnet /opt/apps/minimes/MiniMES.API.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=minimes
User=nginx
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
```

**注意**：Ubuntu/Debian 使用 `User=www-data`

#### 3.6 初始化数据库

```bash
mysql -u root -p
CREATE DATABASE minimes CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
EXIT;
```

系统启动时会自动创建表结构和默认管理员账号。

### 4. 执行部署

在本地项目根目录执行：

```bash
./deploy/deploy.sh
```

脚本会自动完成上传和服务启动。

### 5. 验证部署

访问：`http://your-server-ip:3001`

默认账号：

- 用户名：`admin`
- 密码：`admin123`

⚠️ **首次登录后请立即修改默认密码！**

---

## 🔧 日常更新

### 更新全部

```bash
# 本地构建
cd backend/MiniMES.API && dotnet publish -c Release -o ./publish
cd ../../frontend && npm run build

# 执行部署
cd .. && ./deploy/deploy.sh
```

### 仅更新前端

```bash
cd frontend && npm run build
rsync -avz --delete dist/ user@your-server:/var/www/minimes/
```

### 仅更新后端

```bash
cd backend/MiniMES.API && dotnet publish -c Release -o ./publish
rsync -avz --delete publish/ user@your-server:/opt/apps/minimes/
ssh user@your-server "sudo systemctl restart minimes"
```

---

## 🔍 故障排查

### 查看日志

```bash
# 后端日志
sudo journalctl -u minimes -f

# Nginx 日志
sudo tail -f /var/log/nginx/minimes_error.log
```

### 常见问题

**1. 后端服务无法启动**

```bash
# 检查端口占用
sudo netstat -tlnp | grep 5000

# 手动测试
cd /opt/apps/minimes
dotnet MiniMES.API.dll
```

**2. 数据库连接失败**

```bash
# 测试连接
mysql -h localhost -u root -p minimes

# 检查 MySQL 状态
sudo systemctl status mysql
```

**3. Nginx 502 错误**

```bash
# 检查后端服务
sudo systemctl status minimes

# 测试后端
curl http://localhost:5000/api/health
```

---

## 🔀 多项目共存

如果服务器上已有其他项目，推荐使用不同端口：

```nginx
# 项目 A - 80 端口
server {
    listen 80;
    server_name your-server-ip;
    # ... 项目 A 配置
}

# MiniMES - 3001 端口
server {
    listen 3001;
    server_name your-server-ip;
    # ... MiniMES 配置
}
```

记得开放防火墙端口：

```bash
sudo ufw allow 3001/tcp
```

---

## 🔒 安全加固（可选）

### 配置 HTTPS

```bash
sudo apt install certbot python3-certbot-nginx -y
sudo certbot --nginx -d your-domain.com
```

### 配置防火墙

```bash
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw deny 5000/tcp  # 禁止直接访问后端
sudo ufw enable
```

---

## 📞 支持

如遇问题，请检查：

1. 服务器日志：`sudo journalctl -u minimes -f`
2. Nginx 日志：`sudo tail -f /var/log/nginx/minimes_error.log`
3. 数据库连接：确认 MySQL 正常运行
4. 防火墙规则：确认端口已开放
