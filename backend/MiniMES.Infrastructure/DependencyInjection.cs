using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Application.Interfaces.Services;
using MiniMES.Infrastructure.Data;
using MiniMES.Infrastructure.Repositories;
using MiniMES.Infrastructure.Services;

namespace MiniMES.Infrastructure;

/// <summary>
/// Infrastructure层依赖注入配置
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 注册Infrastructure层所有服务
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 配置数据库
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<MiniMesDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // 注册Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWorkstationRepository, WorkstationRepository>();
        services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
        services.AddScoped<IWorkReportRepository, WorkReportRepository>();

        // 注册Services
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IWorkstationService, WorkstationService>();
        services.AddScoped<IWorkOrderService, WorkOrderService>();
        services.AddScoped<IWorkReportService, WorkReportService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
