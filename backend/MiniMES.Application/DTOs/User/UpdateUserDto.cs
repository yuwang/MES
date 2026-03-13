namespace MiniMES.Application.DTOs.User;

/// <summary>
/// 更新用户DTO
/// </summary>
public class UpdateUserDto
{
    public string RealName { get; set; } = string.Empty;
    public long RoleId { get; set; }
    public int Status { get; set; }
}
