namespace MiniMES.Application.DTOs.User;

/// <summary>
/// 修改密码DTO
/// </summary>
public class ChangePasswordDto
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
