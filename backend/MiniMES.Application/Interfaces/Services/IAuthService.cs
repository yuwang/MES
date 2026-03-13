using MiniMES.Application.DTOs.Auth;
using MiniMES.Shared.Common;

namespace MiniMES.Application.Interfaces.Services;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request);
}
