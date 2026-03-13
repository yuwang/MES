using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MiniMES.Infrastructure.Data;

public class MiniMesDbContextFactory : IDesignTimeDbContextFactory<MiniMesDbContext>
{
    public MiniMesDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MiniMesDbContext>();

        // 获取环境变量
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                        ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                        ?? "Production";

        // 构建配置，优先从环境变量读取
        var basePath = Directory.GetCurrentDirectory();

        // 如果当前目录不是 API 项目，尝试找到 API 项目目录
        if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
        {
            basePath = Path.Combine(basePath, "..", "MiniMES.API");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException($"数据库连接字符串未配置。请在 appsettings.{environment}.json 中配置 ConnectionStrings:DefaultConnection");

        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));

        return new MiniMesDbContext(optionsBuilder.Options);
    }
}
