namespace MiniMES.Application.Interfaces.Services;

/// <summary>
/// 当前用户服务接口
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    long GetUserId();

    /// <summary>
    /// 获取当前用户名
    /// </summary>
    string GetUsername();
}
