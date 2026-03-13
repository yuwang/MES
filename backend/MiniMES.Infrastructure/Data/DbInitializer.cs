using Microsoft.EntityFrameworkCore;
using MiniMES.Domain.Entities;
using MiniMES.Domain.Enums;
using MiniMES.Shared.Utils;

namespace MiniMES.Infrastructure.Data;

/// <summary>
/// 数据库初始化器 - 创建默认角色和管理员账号
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// 初始化数据库种子数据（仅创建角色和管理员账号）
    /// 其他测试数据请使用 docs/init-data.sql 脚本导入
    /// </summary>
    public static async Task InitializeAsync(MiniMesDbContext context)
    {
        // 创建默认角色
        if (!await context.Roles.AnyAsync())
        {
            var roles = new[]
            {
                new Role { RoleName = "Admin", Permissions = "[\"*\"]" },
                new Role { RoleName = "Planner", Permissions = "[\"workorder:read\",\"workorder:write\"]" },
                new Role { RoleName = "Technician", Permissions = "[\"product:read\",\"product:write\",\"workstation:read\",\"workstation:write\"]" },
                new Role { RoleName = "Operator", Permissions = "[\"workreport:read\",\"workreport:write\"]" }
            };
            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        // 创建默认管理员账号 (admin/admin123)
        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.Roles.FirstAsync(r => r.RoleName == "Admin");
            var admin = new User
            {
                Username = "admin",
                PasswordHash = PasswordHasher.HashPassword("admin123"),
                RealName = "系统管理员",
                RoleId = adminRole.Id,
                Status = EntityStatus.Enabled
            };
            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
