namespace MiniMES.Application.DTOs.Device;

/// <summary>
/// 设备实时状态DTO
/// </summary>
public class DeviceStatusDto
{
    /// <summary>
    /// 设备ID
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// 温度（°C）
    /// </summary>
    public double Temperature { get; set; }

    /// <summary>
    /// 转速（RPM）
    /// </summary>
    public double Speed { get; set; }

    /// <summary>
    /// 是否报警
    /// </summary>
    public bool IsAlarming { get; set; }

    /// <summary>
    /// 报警信息
    /// </summary>
    public string AlarmMessage { get; set; } = string.Empty;

    /// <summary>
    /// 数据时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }
}
