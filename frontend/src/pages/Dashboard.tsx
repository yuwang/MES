import { useEffect, useState } from 'react';
import { Card, Row, Col, Statistic, Table, Progress } from 'antd';
import { getProductionProgress } from '../api/dashboard';
import type { ProductionProgressDto } from '../types';

const Dashboard = () => {
  const [loading, setLoading] = useState(false);
  const [progressList, setProgressList] = useState<ProductionProgressDto[]>([]);

  const loadData = async () => {
    setLoading(true);
    try {
      const response = await getProductionProgress();
      if (response.success && response.data) {
        setProgressList(response.data);
      }
    } catch (error) {
      // 错误已在拦截器处理
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const statusMap: Record<number, string> = {
    0: '待产',
    1: '生产中',
    2: '已完工',
    3: '已取消',
  };

  const columns = [
    {
      title: '工单号',
      dataIndex: 'orderNo',
      key: 'orderNo',
    },
    {
      title: '产品名称',
      dataIndex: 'productName',
      key: 'productName',
    },
    {
      title: '目标产量',
      dataIndex: 'targetQuantity',
      key: 'targetQuantity',
    },
    {
      title: '累计良品',
      dataIndex: 'completedQuantity',
      key: 'completedQuantity',
    },
    {
      title: '累计不良品',
      dataIndex: 'defectQuantity',
      key: 'defectQuantity',
    },
    {
      title: '完成率',
      dataIndex: 'completionRate',
      key: 'completionRate',
      render: (rate: number) => <Progress percent={rate} size="small" />,
    },
    {
      title: '良品率',
      dataIndex: 'qualityRate',
      key: 'qualityRate',
      render: (rate: number) => `${rate}%`,
    },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      render: (status: number) => statusMap[status],
    },
  ];

  return (
    <div>
      <h2>生产看板</h2>
      <Row gutter={16} style={{ marginBottom: 24 }}>
        <Col span={6}>
          <Card>
            <Statistic
              title="进行中工单"
              value={progressList.filter(p => p.status === 1).length}
              suffix="个"
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="待产工单"
              value={progressList.filter(p => p.status === 0).length}
              suffix="个"
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="累计良品"
              value={progressList.reduce((sum, p) => sum + p.completedQuantity, 0)}
              suffix="件"
            />
          </Card>
        </Col>
        <Col span={6}>
          <Card>
            <Statistic
              title="累计不良品"
              value={progressList.reduce((sum, p) => sum + p.defectQuantity, 0)}
              suffix="件"
            />
          </Card>
        </Col>
      </Row>

      <Card title="生产进度监控">
        <Table
          loading={loading}
          dataSource={progressList}
          columns={columns}
          rowKey="workOrderId"
          pagination={false}
        />
      </Card>
    </div>
  );
};

export default Dashboard;
