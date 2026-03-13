import { useState, useEffect } from 'react';
import { Table, Button, Space, Modal, Form, InputNumber, message, Popconfirm, Select, Input } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { getWorkReports, createWorkReport, updateWorkReport, deleteWorkReport } from '../../api/workreport';
import { getWorkOrders } from '../../api/workorder';
import { getWorkstations } from '../../api/workstation';
import type { WorkReportDto, CreateWorkReportDto, UpdateWorkReportDto, WorkOrderDto, WorkstationDto } from '../../types';

const WorkReportList = () => {
  const [loading, setLoading] = useState(false);
  const [dataSource, setDataSource] = useState<WorkReportDto[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [workOrderFilter, setWorkOrderFilter] = useState<number | undefined>(undefined);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [workOrders, setWorkOrders] = useState<WorkOrderDto[]>([]);
  const [workstations, setWorkstations] = useState<WorkstationDto[]>([]);
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const response = await getWorkReports({ page, pageSize, workOrderId: workOrderFilter });
      if (response.success && response.data) {
        setDataSource(response.data.items);
        setTotal(response.data.total);
      }
    } catch (error) {
      // 错误已在拦截器处理
    } finally {
      setLoading(false);
    }
  };

  const loadWorkOrders = async () => {
    try {
      const response = await getWorkOrders({ page: 1, pageSize: 1000, status: 1 });
      if (response.success && response.data) {
        setWorkOrders(response.data.items);
      }
    } catch (error) {
      // 错误已在拦截器处理
    }
  };

  const loadWorkstations = async () => {
    try {
      const response = await getWorkstations({ page: 1, pageSize: 1000 });
      if (response.success && response.data) {
        setWorkstations(response.data.items.filter(w => w.status === 1));
      }
    } catch (error) {
      // 错误已在拦截器处理
    }
  };

  useEffect(() => {
    loadData();
  }, [page, workOrderFilter]);

  useEffect(() => {
    loadWorkOrders();
    loadWorkstations();
  }, []);

  const handleAdd = () => {
    setEditingId(null);
    form.resetFields();
    setModalVisible(true);
  };

  const handleEdit = (record: WorkReportDto) => {
    setEditingId(record.id);
    form.setFieldsValue({
      goodQuantity: record.goodQuantity,
      defectQuantity: record.defectQuantity,
      remark: record.remark,
    });
    setModalVisible(true);
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteWorkReport(id);
      message.success('删除成功');
      loadData();
    } catch (error) {
      // 错误已在拦截器处理
    }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (editingId) {
        await updateWorkReport(editingId, values as UpdateWorkReportDto);
        message.success('更新成功');
      } else {
        await createWorkReport(values as CreateWorkReportDto);
        message.success('报工成功');
      }
      setModalVisible(false);
      loadData();
    } catch (error) {
      // 错误已在拦截器处理
    }
  };

  const columns = [
    { title: '工单号', dataIndex: 'orderNo', key: 'orderNo' },
    { title: '产品名称', dataIndex: 'productName', key: 'productName' },
    { title: '工位名称', dataIndex: 'stationName', key: 'stationName' },
    { title: '良品数', dataIndex: 'goodQuantity', key: 'goodQuantity' },
    { title: '不良品数', dataIndex: 'defectQuantity', key: 'defectQuantity' },
    { title: '报工人', dataIndex: 'reportedByName', key: 'reportedByName' },
    {
      title: '报工时间',
      dataIndex: 'reportedAt',
      key: 'reportedAt',
      render: (text: string) => new Date(text).toLocaleString('zh-CN'),
    },
    { title: '备注', dataIndex: 'remark', key: 'remark' },
    {
      title: '操作',
      key: 'action',
      render: (_: any, record: WorkReportDto) => (
        <Space>
          <Button type="link" onClick={() => handleEdit(record)}>编辑</Button>
          <Popconfirm title="确定删除吗？" onConfirm={() => handleDelete(record.id)}>
            <Button type="link" danger>删除</Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between' }}>
        <Select
          placeholder="筛选工单"
          allowClear
          style={{ width: 300 }}
          onChange={setWorkOrderFilter}
          showSearch
          optionFilterProp="children"
          options={workOrders.map(w => ({
            label: `${w.orderNo} - ${w.productName}`,
            value: w.id,
          }))}
        />
        <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
          新增报工
        </Button>
      </div>

      <Table
        loading={loading}
        dataSource={dataSource}
        columns={columns}
        rowKey="id"
        pagination={{
          current: page,
          pageSize,
          total,
          onChange: setPage,
        }}
      />

      <Modal
        title={editingId ? '编辑报工' : '新增报工'}
        open={modalVisible}
        onOk={handleSubmit}
        onCancel={() => setModalVisible(false)}
      >
        <Form form={form} layout="vertical">
          {!editingId && (
            <>
              <Form.Item
                name="workOrderId"
                label="工单"
                rules={[{ required: true, message: '请选择工单' }]}
              >
                <Select
                  placeholder="请选择工单"
                  showSearch
                  optionFilterProp="children"
                  options={workOrders.map(w => ({
                    label: `${w.orderNo} - ${w.productName}`,
                    value: w.id,
                  }))}
                />
              </Form.Item>
              <Form.Item
                name="workstationId"
                label="工位"
                rules={[{ required: true, message: '请选择工位' }]}
              >
                <Select
                  placeholder="请选择工位"
                  showSearch
                  optionFilterProp="children"
                  options={workstations.map(w => ({
                    label: `${w.stationCode} - ${w.stationName}`,
                    value: w.id,
                  }))}
                />
              </Form.Item>
            </>
          )}
          <Form.Item
            name="goodQuantity"
            label="良品数"
            rules={[{ required: true, message: '请输入良品数' }]}
          >
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item
            name="defectQuantity"
            label="不良品数"
            rules={[{ required: true, message: '请输入不良品数' }]}
          >
            <InputNumber min={0} style={{ width: '100%' }} />
          </Form.Item>
          <Form.Item name="remark" label="备注">
            <Input.TextArea rows={3} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default WorkReportList;
