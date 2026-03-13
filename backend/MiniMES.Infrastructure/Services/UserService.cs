using Microsoft.EntityFrameworkCore;
using MiniMES.Application.DTOs.User;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Application.Interfaces.Services;
using MiniMES.Domain.Entities;
using MiniMES.Domain.Enums;
using MiniMES.Infrastructure.Data;
using MiniMES.Shared.Common;
using MiniMES.Shared.Utils;

namespace MiniMES.Infrastructure.Services;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly MiniMesDbContext _context;

    public UserService(IUserRepository userRepository, MiniMesDbContext context)
    {
        _userRepository = userRepository;
        _context = context;
    }

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    public async Task<ApiResponse<PagedResponse<UserDto>>> GetPagedAsync(int page, int pageSize, string? keyword)
    {
        var (items, total) = await _userRepository.GetPagedAsync(page, pageSize, keyword);

        var userDtos = items.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            RealName = u.RealName,
            RoleId = u.RoleId,
            RoleName = u.Role.RoleName,
            Status = (int)u.Status,
            CreatedAt = u.CreatedAt,
            UpdatedAt = u.UpdatedAt
        }).ToList();

        var pagedResponse = new PagedResponse<UserDto>
        {
            Items = userDtos,
            Total = total,
            Page = page,
            PageSize = pageSize
        };

        return ApiResponse<PagedResponse<UserDto>>.SuccessResult(pagedResponse);
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    public async Task<ApiResponse<UserDto>> GetByIdAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<UserDto>.FailResult("用户不存在");
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            RoleId = user.RoleId,
            RoleName = user.Role.RoleName,
            Status = (int)user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };

        return ApiResponse<UserDto>.SuccessResult(userDto);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    public async Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto dto)
    {
        // 检查用户名是否已存在
        var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUser != null)
        {
            return ApiResponse<UserDto>.FailResult("用户名已存在");
        }

        // 检查角色是否存在
        var role = await _context.Roles.FindAsync(dto.RoleId);
        if (role == null)
        {
            return ApiResponse<UserDto>.FailResult("角色不存在");
        }

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = PasswordHasher.HashPassword(dto.Password),
            RealName = dto.RealName,
            RoleId = dto.RoleId,
            Status = EntityStatus.Enabled
        };

        await _userRepository.CreateAsync(user);

        // 重新查询以获取关联的角色信息
        var createdUser = await _userRepository.GetByIdAsync(user.Id);

        var userDto = new UserDto
        {
            Id = createdUser!.Id,
            Username = createdUser.Username,
            RealName = createdUser.RealName,
            RoleId = createdUser.RoleId,
            RoleName = createdUser.Role.RoleName,
            Status = (int)createdUser.Status,
            CreatedAt = createdUser.CreatedAt,
            UpdatedAt = createdUser.UpdatedAt
        };

        return ApiResponse<UserDto>.SuccessResult(userDto);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    public async Task<ApiResponse<UserDto>> UpdateAsync(long id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdForUpdateAsync(id);
        if (user == null)
        {
            return ApiResponse<UserDto>.FailResult("用户不存在");
        }

        // 检查角色是否存在
        var role = await _context.Roles.FindAsync(dto.RoleId);
        if (role == null)
        {
            return ApiResponse<UserDto>.FailResult("角色不存在");
        }

        user.RealName = dto.RealName;
        user.RoleId = dto.RoleId;
        user.Status = (EntityStatus)dto.Status;

        await _userRepository.UpdateAsync(user);

        // 重新查询以获取关联的角色信息
        var updatedUser = await _userRepository.GetByIdAsync(user.Id);

        var userDto = new UserDto
        {
            Id = updatedUser!.Id,
            Username = updatedUser.Username,
            RealName = updatedUser.RealName,
            RoleId = updatedUser.RoleId,
            RoleName = updatedUser.Role.RoleName,
            Status = (int)updatedUser.Status,
            CreatedAt = updatedUser.CreatedAt,
            UpdatedAt = updatedUser.UpdatedAt
        };

        return ApiResponse<UserDto>.SuccessResult(userDto);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    public async Task<ApiResponse<bool>> DeleteAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<bool>.FailResult("用户不存在");
        }

        // 不允许删除admin用户
        if (user.Username == "admin")
        {
            return ApiResponse<bool>.FailResult("不允许删除管理员账号");
        }

        var result = await _userRepository.DeleteAsync(id);
        return result
            ? ApiResponse<bool>.SuccessResult(true)
            : ApiResponse<bool>.FailResult("删除失败");
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    public async Task<ApiResponse<bool>> ChangePasswordAsync(long id, ChangePasswordDto dto)
    {
        var user = await _userRepository.GetByIdForUpdateAsync(id);
        if (user == null)
        {
            return ApiResponse<bool>.FailResult("用户不存在");
        }

        // 验证旧密码
        if (!PasswordHasher.VerifyPassword(dto.OldPassword, user.PasswordHash))
        {
            return ApiResponse<bool>.FailResult("旧密码错误");
        }

        user.PasswordHash = PasswordHasher.HashPassword(dto.NewPassword);
        await _userRepository.UpdateAsync(user);

        return ApiResponse<bool>.SuccessResult(true);
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    public async Task<ApiResponse<bool>> ResetPasswordAsync(long id, string newPassword)
    {
        var user = await _userRepository.GetByIdForUpdateAsync(id);
        if (user == null)
        {
            return ApiResponse<bool>.FailResult("用户不存在");
        }

        user.PasswordHash = PasswordHasher.HashPassword(newPassword);
        await _userRepository.UpdateAsync(user);

        return ApiResponse<bool>.SuccessResult(true);
    }
}
