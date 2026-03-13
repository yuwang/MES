using System.ComponentModel.DataAnnotations;

namespace MiniMES.Application.DTOs.Auth;

/// <summary>
/// 登录请求DTO
/// </summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;
}
