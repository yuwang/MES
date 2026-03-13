import { useState, useEffect } from "react";
import {
  Table,
  Button,
  Space,
  Modal,
  Form,
  Input,
  message,
  Popconfirm,
  Tag,
  Select,
} from "antd";
import { PlusOutlined } from "@ant-design/icons";
import { getUsers, createUser, updateUser, deleteUser } from "../../api/user";
import type { UserDto, CreateUserDto, UpdateUserDto } from "../../types";

const UserList = () => {
  const [loading, setLoading] = useState(false);
  const [dataSource, setDataSource] = useState<UserDto[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(10);
  const [keyword, setKeyword] = useState("");
  const [modalVisible, setModalVisible] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [form] = Form.useForm();

  const roles = [
    { id: 1, name: "Admin", label: "管理员" },
    { id: 2, name: "Planner", label: "计划员" },
    { id: 3, name: "Technician", label: "技术员" },
    { id: 4, name: "Operator", label: "操作工" },
  ];

  const getRoleLabel = (roleName: string) => {
    const role = roles.find(r => r.name === roleName);
    return role ? role.label : roleName;
  };

  const loadData = async () => {
    setLoading(true);
    try {
      const response = await getUsers({ page, pageSize, keyword });
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

  const handleEdit = (record: UserDto) => {
    setEditingId(record.id);
    form.setFieldsValue({
      realName: record.realName,
      roleId: record.roleId,
      status: record.status,
    });
    setModalVisible(true);
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteUser(id);
      message.success("删除成功");
      loadData();
    } catch (error) {
      // 错误已在拦截器处理
    }
  };

  const handleSubmit = async () => {
    try {
      const values = await form.validateFields();
      if (editingId) {
        await updateUser(editingId, values as UpdateUserDto);
        message.success("更新成功");
      } else {
        await createUser(values as CreateUserDto);
        message.success("创建成功");
      }
      setModalVisible(false);
      loadData();
    } catch (error) {
      // 错误已在拦截器处理
    }
  };

  const columns = [
    { title: "用户名", dataIndex: "username", key: "username" },
    { title: "姓名", dataIndex: "realName", key: "realName" },
    {
      title: "角色",
      dataIndex: "roleName",
      key: "roleName",
      render: (roleName: string) => getRoleLabel(roleName)
    },
    {
      title: "状态",
      dataIndex: "status",
      key: "status",
      render: (status: number) => (
        <Tag color={status === 1 ? "green" : "red"}>
          {status === 1 ? "启用" : "停用"}
        </Tag>
      ),
    },
    {
      title: "创建时间",
      dataIndex: "createdAt",
      key: "createdAt",
      render: (text: string) => new Date(text).toLocaleString("zh-CN"),
    },
    {
      title: "操作",
      key: "action",
      render: (_: any, record: UserDto) => (
        <Space>
          <Button type="link" onClick={() => handleEdit(record)}>
            编辑
          </Button>
          {record.username !== "admin" && (
            <Popconfirm
              title="确定删除吗？"
              onConfirm={() => handleDelete(record.id)}
            >
              <Button type="link" danger>
                删除
              </Button>
            </Popconfirm>
          )}
        </Space>
      ),
    },
  ];

  return (
    <div>
      <div
        style={{
          marginBottom: 16,
          display: "flex",
          justifyContent: "space-between",
        }}
      >
        <Input.Search
          placeholder="搜索用户名或姓名"
          onSearch={setKeyword}
          style={{ width: 300 }}
        />
        <Button type="primary" icon={<PlusOutlined />} onClick={handleAdd}>
          新增用户
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
        title={editingId ? "编辑用户" : "新增用户"}
        open={modalVisible}
        onOk={handleSubmit}
        onCancel={() => setModalVisible(false)}
      >
        <Form form={form} layout="vertical">
          {!editingId && (
            <>
              <Form.Item
                name="username"
                label="用户名"
                rules={[{ required: true, message: "请输入用户名" }]}
              >
                <Input />
              </Form.Item>
              <Form.Item
                name="password"
                label="密码"
                rules={[{ required: true, message: "请输入密码" }]}
              >
                <Input.Password />
              </Form.Item>
            </>
          )}
          <Form.Item
            name="realName"
            label="姓名"
            rules={[{ required: true, message: "请输入姓名" }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="roleId"
            label="角色"
            rules={[{ required: true, message: "请选择角色" }]}
          >
            <Select
              options={roles.map((r) => ({
                label: r.label,
                value: r.id,
              }))}
            />
          </Form.Item>
          {editingId && (
            <Form.Item name="status" label="状态" rules={[{ required: true }]}>
              <Select
                options={[
                  { label: "启用", value: 1 },
                  { label: "停用", value: 0 },
                ]}
              />
            </Form.Item>
          )}
        </Form>
      </Modal>
    </div>
  );
};

export default UserList;
