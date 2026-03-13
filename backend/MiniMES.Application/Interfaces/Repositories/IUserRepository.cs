using MiniMES.Domain.Entities;

namespace MiniMES.Application.Interfaces.Repositories;

/// <summary>
/// 用户仓储接口
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// 根据用户名获取用户
    /// </summary>
    Task<User?> GetByUsernameAsync(string username);

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    Task<User?> GetByIdAsync(long id);

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    Task<(List<User> Items, int Total)> GetPagedAsync(int page, int pageSize, string? keyword);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task<bool> DeleteAsync(long id);

    /// <summary>
    /// 根据ID获取用户（用于更新，需要跟踪）
    /// </summary>
    Task<User?> GetByIdForUpdateAsync(long id);
}
