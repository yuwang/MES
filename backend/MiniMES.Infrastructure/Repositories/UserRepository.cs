using Microsoft.EntityFrameworkCore;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Data;

namespace MiniMES.Infrastructure.Repositories;

/// <summary>
/// 用户仓储实现
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly MiniMesDbContext _context;

    public UserRepository(MiniMesDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 根据用户名获取用户（包含角色信息）
    /// </summary>
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// 根据ID获取用户（包含角色信息）
    /// </summary>
    public async Task<User?> GetByIdAsync(long id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    public async Task<(List<User> Items, int Total)> GetPagedAsync(int page, int pageSize, string? keyword)
    {
        var query = _context.Users
            .Include(u => u.Role)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(u => u.Username.Contains(keyword) || u.RealName.Contains(keyword));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    public async Task<User> UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 根据ID获取用户（用于更新，需要跟踪）
    /// </summary>
    public async Task<User?> GetByIdForUpdateAsync(long id)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id);
    }
}
