using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MiniMES.API.Hubs;

/// <summary>
/// 设备实时监控 SignalR Hub
/// </summary>
[Authorize]
public class DeviceMonitorHub : Hub
{
    /// <summary>
    /// 客户端连接时自动加入 all-devices 组
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "all-devices");
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// 订阅指定设备的数据推送
    /// </summary>
    public async Task SubscribeDevice(string deviceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"device-{deviceId}");
    }

    /// <summary>
    /// 取消订阅指定设备
    /// </summary>
    public async Task UnsubscribeDevice(string deviceId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"device-{deviceId}");
    }
}
