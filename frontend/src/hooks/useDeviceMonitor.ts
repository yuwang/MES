import { useEffect, useRef, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import type { DeviceStatusDto } from '../types';

const HUB_URL = '/hubs/device-monitor';
const MAX_HISTORY = 60; // 每台设备保留最近 60 条数据

export interface DeviceHistory {
  latest: DeviceStatusDto;
  history: DeviceStatusDto[];
}

export function useDeviceMonitor() {
  const [devices, setDevices] = useState<Map<string, DeviceHistory>>(new Map());
  const [connected, setConnected] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL, {
        accessTokenFactory: () => localStorage.getItem('token') ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    connection.on('ReceiveDeviceStatus', (dataList: DeviceStatusDto[]) => {
      setDevices(prev => {
        const next = new Map(prev);
        for (const item of dataList) {
          const existing = next.get(item.deviceId);
          const history = existing ? [...existing.history, item].slice(-MAX_HISTORY) : [item];
          next.set(item.deviceId, { latest: item, history });
        }
        return next;
      });
    });

    connection.onreconnecting(() => setConnected(false));
    connection.onreconnected(() => setConnected(true));
    connection.onclose(() => setConnected(false));

    connection.start()
      .then(() => { setConnected(true); setError(null); })
      .catch(err => setError(err?.message ?? '连接失败'));

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, []);

  return { devices, connected, error };
}
