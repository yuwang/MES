import { useState, useEffect } from 'react';
import { Table, Button, Space, Modal, Form, Input, message, Popconfirm, Tag } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { getWorkstations, createWorkstation, updateWorkstation, deleteWorkstation } from '../../api/workstation';
import type { WorkstationDto, CreateWorkstationDto, UpdateWorkstationDto } from '../../types';

const WorkstationList = () => {
  const [loading, setLoading] = useState(false);
  const [dataSource, setDataSource] = useState<WorkstationDto[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [keyword, setKeyword] = useState('');
  const [modalVisible, setModalVisible] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [form] = Form.useForm();

  const loadData = async () => {
    setLoading(true);
    try {
      const response = await getWorkstations({ page, pageSize, keyword });
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

  useEffect(() => {
    loadData();
  }, [page, keyword]);

  const handleAdd = () => {
    setEditingId(null);
    form.resetFields();
    setModalVisible(true);
  };

  const handleEdit = (record: WorkstationDto) => {
    setEditingId(record.id);
    form.setFieldsValue(record);
    setModalVisible(true);
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteWorkstation(id);
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
        const { stationCode, ...updateData } = values;
        await updateWorkstation(editingId, updateData as UpdateWorkstationDto);
        message.success('更新成功');
      } else {
        await createWorkstation(values as CreateWorkstationDto);
        message.success('创建成功');
      }
      setModalVisible(false);
      loadData();
    } catch (error) {
      // 错误已在拦截器处理
    }
  };

  const columns = [
    { title: '工位编号', dataIndex: 'stationCode', key: 'stationCode' },
    { title: '工位名称', dataIndex: 'stationName', key: 'stationName' },
    { title: '所属车间', dataIndex: 'workshop', key: 'workshop' },
    {
      title: '状态',
      dataIndex: 'status',
      key: 'status',
      render: (status: number) => (
        <Tag color={status === 1 ? 'green' : 'red'}>
          {status === 1 ? '启用' : '停用'}
        </Tag>
      ),
    },
    {
      title: '操作',
      key: 'action',
      render: (_: any, record: WorkstationDto) => (
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
        <Input.Search
          placeholder="搜索工位编号或名称"
          onSearch={setKeyword}
          style={{ width: 300 }}
        />
        <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
          新增工位
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
        title={editingId ? '编辑工位' : '新增工位'}
        open={modalVisible}
        onOk={handleSubmit}
        onCancel={() => setModalVisible(false)}
      >
        <Form form={form} layout="vertical">
          <Form.Item
            name="stationCode"
            label="工位编号"
            rules={[{ required: true, message: '请输入工位编号' }]}
          >
            <Input disabled={!!editingId} />
          </Form.Item>
          <Form.Item
            name="stationName"
            label="工位名称"
            rules={[{ required: true, message: '请输入工位名称' }]}
          >
            <Input />
          </Form.Item>
          <Form.Item name="workshop" label="所属车间">
            <Input />
          </Form.Item>
          {editingId && (
            <Form.Item
              name="status"
              label="状态"
              rules={[{ required: true }]}
            >
              <Input type="number" min={0} max={1} />
            </Form.Item>
          )}
        </Form>
      </Modal>
    </div>
  );
};

export default WorkstationList;
