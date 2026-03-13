using MiniMES.Application.DTOs.Auth;
using MiniMES.Application.Interfaces.Repositories;
using MiniMES.Application.Interfaces.Services;
using MiniMES.Domain.Enums;
using MiniMES.Shared.Common;
using MiniMES.Shared.Utils;

namespace MiniMES.Infrastructure.Services;

/// <summary>
/// 认证服务 - 处理用户登录和身份验证
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        // 验证用户名和密码
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return ApiResponse<LoginResponseDto>.FailResult("用户名或密码错误");
        }

        // 检查账号状态
        if (user.Status == EntityStatus.Disabled)
        {
            return ApiResponse<LoginResponseDto>.FailResult("账号已被停用");
        }

        // 检查角色配置
        if (user.Role == null)
        {
            return ApiResponse<LoginResponseDto>.FailResult("用户角色未配置");
        }

        // 生成 JWT Token
        var token = _tokenService.GenerateToken(user, user.Role);

        var response = new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            RealName = user.RealName,
            RoleName = user.Role.RoleName
        };

        return ApiResponse<LoginResponseDto>.SuccessResult(response);
    }
}
