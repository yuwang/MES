using Microsoft.AspNetCore.SignalR;
using MiniMES.Application.DTOs.Device;

namespace MiniMES.API.BackgroundServices;

/// <summary>
/// 设备数据模拟器 — 每秒生成 3 台设备的随机状态并通过 SignalR 推送
/// </summary>
public class DeviceDataSimulator : BackgroundService
{
    private readonly IHubContext<Hubs.DeviceMonitorHub> _hubContext;
    private readonly ILogger<DeviceDataSimulator> _logger;
    private readonly Random _random = new();

    private static readonly (string Id, string Name)[] Devices =
    [
        ("DEV-001", "注塑机 #1"),
        ("DEV-002", "冲压机 #2"),
        ("DEV-003", "装配线 #3"),
    ];

    public DeviceDataSimulator(
        IHubContext<Hubs.DeviceMonitorHub> hubContext,
        ILogger<DeviceDataSimulator> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("设备数据模拟器已启动");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var dataList = Devices.Select(d => GenerateDeviceStatus(d.Id, d.Name)).ToList();
                await _hubContext.Clients.Group("all-devices")
                    .SendAsync("ReceiveDeviceStatus", dataList, stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "推送设备数据时发生错误");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }

        _logger.LogInformation("设备数据模拟器已停止");
    }

    private DeviceStatusDto GenerateDeviceStatus(string deviceId, string deviceName)
    {
        var temperature = Math.Round(60 + _random.NextDouble() * 40, 1); // 60~100 °C
        var speed = Math.Round(1000 + _random.NextDouble() * 2000, 0);   // 1000~3000 RPM

        var alarms = new List<string>();
        if (temperature > 90) alarms.Add($"温度过高 ({temperature}°C)");
        if (speed > 2800) alarms.Add($"转速过高 ({speed} RPM)");

        return new DeviceStatusDto
        {
            DeviceId = deviceId,
            DeviceName = deviceName,
            Temperature = temperature,
            Speed = speed,
            IsAlarming = alarms.Count > 0,
            AlarmMessage = string.Join("; ", alarms),
            Timestamp = DateTime.UtcNow,
        };
    }
}
