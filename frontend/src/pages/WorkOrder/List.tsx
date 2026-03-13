import { useState, useEffect } from 'react';
import { Table, Button, Space, Modal, Form, Input, InputNumber, message, Popconfirm, Tag, Select } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { getWorkOrders, createWorkOrder, updateWorkOrder, cancelWorkOrder, deleteWorkOrder } from '../../api/workorder';
import { getProducts } from '../../api/product';
import type { WorkOrderDto, CreateWorkOrderDto, UpdateWorkOrderDto, ProductDto } from '../../types';

const WorkOrderList = () => {
  const [loading, setLoading] = useState(false);
  const [dataSource, setDataSource] = useState<WorkOrderDto[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [keyword, setKeyword] = useState('');
  const [statusFilter, setStatusFilter] = useState<number | undefined>(undefined);
  const [modalVisible, setModalVisible] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const response = await getWorkOrders({ page, pageSize, keyword, status: statusFilter });
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

  const loadProducts = async () => {
    try {
      const response = await getProducts({ page: 1, pageSize: 1000 });
      if (response.success && response.data) {
        setProducts(response.data.items.filter(p => p.status === 1));
      }
    } catch (error) {
      // 错误已在拦截器处理
    }
  };

  useEffect(() => {
    loadData();
  }, [page, keyword, statusFilter]);

  useEffect(() => {
    loadProducts();
  }, []);

  const handleAdd = () => {
    setEditingId(null);
    form.resetFields();
    setModalVisible(true);
  };

  const handleEdit = (record: WorkOrderDto) => {
    setEditingId(record.id);
    form.setFieldsValue({
      targetQuantity: record.targetQuantity,
    });
    setModalVisible(true);
  };

  const handleCancel = async (id: number) => {
    try {
      await cancelWorkOrder(id);
      message.success('取消成功');
      loadData();
    } catch (error) {
      // 错误已在拦截器处理
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteWorkOrder(id);
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
        await updateWorkOrder(editingId, values as UpdateWorkOrderDto);
        message.success('更新成功');
      } else {
        await createWorkOrder(values as CreateWorkOrderDto);
        message.success('创建成功');
      }
      setModalVisible(false);
      loadData();
    } catch (error) {
      // 错误已在拦截器处理
    }
  };

  const statusMap: Record<number, { text: string; color: string }> = {
    0: { text: '待产', color: 'default' },
    1: { text: '生产中', color: 'processing' },
    2: { text: '已完工', color: 'success' },
    3: { text: '已取消', color: 'error' },
  };

  const columns = [
    { title: '工单号', dataIndex: 'orderNo', key: 'orderNo' },
    { title: '产品编码', dataIndex: 'productCode', key: 'productCode' },
    { title: '产品名称', dataIndex: 'productName', key: 'productName' },
    { title: '目标产量', dataIndex: 'targetQuantity', key: 'targetQuantity' },
    { title: '累计良品', dataIndex: 'completedQuantity', key: 'completedQuantity' },
    { title: '累计不良品', dataIndex: 'defectQuantity', key: 'defectQuantity' },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      render: (status: number) => (
        <Tag color={statusMap[status]?.color}>
          {statusMap[status]?.text}
        </Tag>
      ),
    },
    { title: '创建人', dataIndex: 'createdByName', key: 'createdByName' },
    {
      title: '操作',
      key: 'action',
      render: (_: any, record: WorkOrderDto) => (
        <Space>
          {record.status === 0 && (
            <Button type="link" onClick={() => handleEdit(record)}>编辑</Button>
          )}
          {(record.status === 0 || record.status === 1) && (
            <Popconfirm title="确定取消吗？" onConfirm={() => handleCancel(record.id)}>
              <Button type="link" danger>取消</Button>
            </Popconfirm>
          )}
          {record.status === 0 && (
            <Popconfirm title="确定删除吗？" onConfirm={() => handleDelete(record.id)}>
              <Button type="link" danger>删除</Button>
            </Popconfirm>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div style={{ marginBottom: 16, display: 'flex', justifyContent: 'space-between' }}>
        <Space>
          <Input.Search
            placeholder="搜索工单号或产品名称"
            onSearch={setKeyword}
            style={{ width: 300 }}
          />
          <Select
            placeholder="筛选状态"
            allowClear
            style={{ width: 120 }}
            onChange={setStatusFilter}
            options={[
              { label: '待产', value: 0 },
              { label: '生产中', value: 1 },
              { label: '已完工', value: 2 },
              { label: '已取消', value: 3 },
            ]}
          />
        </Space>
        <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
          新增工单
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
        title={editingId ? '编辑工单' : '新增工单'}
        open={modalVisible}
        onOk={handleSubmit}
        onCancel={() => setModalVisible(false)}
      >
        <Form form={form} layout="vertical">
          {!editingId && (
            <>
              <Form.Item
                name="orderNo"
                label="工单号"
                rules={[{ required: true, message: '请输入工单号' }]}
              >
                <Input placeholder="例如: WO20260312001" />
              </Form.Item>
              <Form.Item
                name="productId"
                label="产品"
                rules={[{ required: true, message: '请选择产品' }]}
              >
                <Select
                  placeholder="请选择产品"
                  showSearch
                  optionFilterProp="children"
                  options={products.map(p => ({
                    label: `${p.productCode} - ${p.productName}`,
                    value: p.id,
                  }))}
                />
              </Form.Item>
            </>
          )}
          <Form.Item
            name="targetQuantity"
            label="目标产量"
            rules={[{ required: true, message: '请输入目标产量' }]}
          >
            <InputNumber min={1} style={{ width: '100%' }} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
};

export default WorkOrderList;
