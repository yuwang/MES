using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MiniMES.Application.Interfaces.Services;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Services;

/// <summary>
/// JWT Token 服务 - 生成和管理 JWT 身份令牌
/// </summary>
public class JwtTokenService : ITokenService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationHours;

    public JwtTokenService(IConfiguration configuration)
    {
        _key = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
        _issuer = configuration["Jwt:Issuer"] ?? "MiniMES";
        _audience = configuration["Jwt:Audience"] ?? "MiniMES";
        _expirationHours = int.Parse(configuration["Jwt:ExpirationHours"] ?? "8");
    }

    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    public string GenerateToken(User user, Role role)
    {
        // 构建用户声明
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, role.RoleName)
        };

        // 创建签名密钥
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 生成 Token
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_expirationHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
