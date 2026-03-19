import { Badge, Card, Col, Row, Statistic, Tag, Alert } from "antd";
import ReactECharts from "echarts-for-react";
import { useDeviceMonitor } from "../../hooks/useDeviceMonitor";
import type { DeviceStatusDto } from "../../types";

function buildChartOption(history: DeviceStatusDto[]) {
  const times = history.map((d) => new Date(d.timestamp).toLocaleTimeString());
  const temps = history.map((d) => d.temperature);
  const speeds = history.map((d) => d.speed);

  return {
    animation: false,
    tooltip: { trigger: "axis" },
    legend: { data: ["温度(°C)", "转速(RPM)"] },
    grid: { left: 60, right: 60, bottom: 70, top: 40 },
    xAxis: {
      type: "category",
      data: times,
      axisLabel: { rotate: 30, fontSize: 10 },
    },
    yAxis: [
      { type: "value", name: "温度(°C)", min: 50, max: 110 },
      { type: "value", name: "转速(RPM)", min: 800, max: 3200 },
    ],
    series: [
      {
        name: "温度(°C)",
        type: "line",
        data: temps,
        smooth: true,
        itemStyle: { color: "#ff4d4f" },
        markLine: {
          data: [
            {
              yAxis: 90,
              name: "报警阈值",
              lineStyle: { color: "#ff4d4f", type: "dashed" },
            },
          ],
        },
      },
      {
        name: "转速(RPM)",
        type: "line",
        yAxisIndex: 1,
        data: speeds,
        smooth: true,
        itemStyle: { color: "#1677ff" },
        markLine: {
          data: [
            {
              yAxis: 2800,
              name: "报警阈值",
              lineStyle: { color: "#1677ff", type: "dashed" },
            },
          ],
        },
      },
    ],
  };
}

const DeviceMonitor = () => {
  const { devices, connected, error } = useDeviceMonitor();

  return (
    <div>
      <div
        style={{
          display: "flex",
          alignItems: "center",
          gap: 12,
          marginBottom: 16,
        }}
      >
        <h2 style={{ margin: 0 }}>设备实时监控</h2>
        <Badge
          status={connected ? "success" : "error"}
          text={connected ? "WebSocket 已连接" : "WebSocket 未连接"}
        />
      </div>

      {error && (
        <Alert type="error" message={error} style={{ marginBottom: 16 }} />
      )}

      {devices.size === 0 && connected && (
        <Alert
          type="info"
          message="等待设备数据推送..."
          style={{ marginBottom: 16 }}
        />
      )}

      <Row gutter={[16, 16]}>
        {Array.from(devices.values()).map(({ latest, history }) => (
          <Col key={latest.deviceId} xs={24} xl={8}>
            <Card
              title={
                <span>
                  {latest.deviceName}
                  <Tag
                    color={latest.isAlarming ? "error" : "success"}
                    style={{ marginLeft: 8 }}
                  >
                    {latest.isAlarming ? "报警" : "正常"}
                  </Tag>
                </span>
              }
              extra={
                <span style={{ fontSize: 12, color: "#999" }}>
                  {latest.deviceId}
                </span>
              }
            >
              <div
                style={{
                  height: 40,
                  marginBottom: 12,
                  visibility: latest.isAlarming ? "visible" : "hidden",
                }}
              >
                <Alert
                  type="error"
                  message={latest.alarmMessage}
                  showIcon
                  style={{
                    overflow: "hidden",
                    whiteSpace: "nowrap",
                    textOverflow: "ellipsis",
                  }}
                />
              </div>
              <Row gutter={16} style={{ marginBottom: 12 }}>
                <Col span={12}>
                  <Statistic
                    title="当前温度"
                    value={latest.temperature}
                    suffix="°C"
                    precision={1}
                    valueStyle={{
                      color: latest.temperature > 90 ? "#ff4d4f" : undefined,
                    }}
                  />
                </Col>
                <Col span={12}>
                  <Statistic
                    title="当前转速"
                    value={latest.speed}
                    suffix="RPM"
                    precision={0}
                    valueStyle={{
                      color: latest.speed > 2800 ? "#ff4d4f" : undefined,
                    }}
                  />
                </Col>
              </Row>
              <ReactECharts
                option={buildChartOption(history)}
                style={{ height: 220 }}
                notMerge
              />
            </Card>
          </Col>
        ))}
      </Row>
    </div>
  );
};

export default DeviceMonitor;
