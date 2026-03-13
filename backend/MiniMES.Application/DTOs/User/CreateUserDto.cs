namespace MiniMES.Application.DTOs.User;

/// <summary>
/// 创建用户DTO
/// </summary>
public class CreateUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public long RoleId { get; set; }
}
