namespace MiniMES.Domain.Entities;

public class Role : BaseEntity
{
    public string RoleName { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty; // JSON 格式存储权限列表

    // 导航属性
    public ICollection<User> Users { get; set; } = new List<User>();
}
