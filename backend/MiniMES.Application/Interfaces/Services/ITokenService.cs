using MiniMES.Domain.Entities;

namespace MiniMES.Application.Interfaces.Services;

/// <summary>
/// Token服务接口
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// 生成JWT Token
    /// </summary>
    string GenerateToken(User user, Role role);
}
