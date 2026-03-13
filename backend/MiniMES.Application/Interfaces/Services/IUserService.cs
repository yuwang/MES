using MiniMES.Application.DTOs.User;
using MiniMES.Shared.Common;

namespace MiniMES.Application.Interfaces.Services;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    Task<ApiResponse<PagedResponse<UserDto>>> GetPagedAsync(int page, int pageSize, string? keyword);

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    Task<ApiResponse<UserDto>> GetByIdAsync(long id);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto dto);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task<ApiResponse<UserDto>> UpdateAsync(long id, UpdateUserDto dto);

    /// <summary>
    /// 删除用户
    /// </summary>
    Task<ApiResponse<bool>> DeleteAsync(long id);

    /// <summary>
    /// 修改密码
    /// </summary>
    Task<ApiResponse<bool>> ChangePasswordAsync(long id, ChangePasswordDto dto);

    /// <summary>
    /// 重置密码
    /// </summary>
    Task<ApiResponse<bool>> ResetPasswordAsync(long id, string newPassword);
}
