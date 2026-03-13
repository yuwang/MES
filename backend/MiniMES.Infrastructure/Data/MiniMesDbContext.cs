using Microsoft.EntityFrameworkCore;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Data;

public class MiniMesDbContext : DbContext
{
    public MiniMesDbContext(DbContextOptions<MiniMesDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Workstation> Workstations { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<WorkReport> WorkReports { get; set; }
    public DbSet<OperationLog> OperationLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User 配置
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.RealName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Status).HasConversion<int>();
            entity.HasOne(e => e.Role).WithMany(r => r.Users).HasForeignKey(e => e.RoleId);
        });

        // Role 配置
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RoleName).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.RoleName).IsUnique();
            entity.Property(e => e.Permissions).HasColumnType("json");
        });

        // Product 配置
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ProductCode).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.ProductCode).IsUnique();
            entity.Property(e => e.ProductName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Specification).HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<int>();
        });

        // Workstation 配置
        modelBuilder.Entity<Workstation>(entity =>
        {
            entity.ToTable("workstations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StationCode).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.StationCode).IsUnique();
            entity.Property(e => e.StationName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Workshop).HasMaxLength(100);
            entity.Property(e => e.Status).HasConversion<int>();
        });

        // WorkOrder 配置
        modelBuilder.Entity<WorkOrder>(entity =>
        {
            entity.ToTable("work_orders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNo).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.OrderNo).IsUnique();
            entity.Property(e => e.Status).HasConversion<int>();
            entity.HasOne(e => e.Product).WithMany(p => p.WorkOrders).HasForeignKey(e => e.ProductId);
            entity.HasOne(e => e.Creator).WithMany(u => u.WorkOrders).HasForeignKey(e => e.CreatedBy);
        });

        // WorkReport 配置
        modelBuilder.Entity<WorkReport>(entity =>
        {
            entity.ToTable("work_reports");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.WorkOrder).WithMany(wo => wo.WorkReports).HasForeignKey(e => e.WorkOrderId);
            entity.HasOne(e => e.Workstation).WithMany(ws => ws.WorkReports).HasForeignKey(e => e.WorkstationId);
            entity.HasOne(e => e.Reporter).WithMany(u => u.WorkReports).HasForeignKey(e => e.ReportedBy);
        });

        // OperationLog 配置
        modelBuilder.Entity<OperationLog>(entity =>
        {
            entity.ToTable("operation_logs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OperationType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.OperationDesc).HasMaxLength(500);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
        });
    }
}
