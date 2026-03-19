import { useEffect } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import { Layout as AntLayout, Menu, Dropdown, Avatar } from 'antd';
import {
  DashboardOutlined,
  AppstoreOutlined,
  EnvironmentOutlined,
  FileTextOutlined,
  FormOutlined,
  UserOutlined,
  LogoutOutlined,
  TeamOutlined,
  MonitorOutlined,
} from '@ant-design/icons';
import { useAuthStore } from '../stores/auth';

const { Header, Sider, Content } = AntLayout;

const Layout = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { userInfo, isAuthenticated, clearAuth } = useAuthStore();

  useEffect(() => {
    if (!isAuthenticated()) {
      navigate('/login');
    }
  }, [isAuthenticated, navigate]);

  const handleLogout = () => {
    clearAuth();
    navigate('/login');
  };

  const menuItems = [
    {
      key: '/dashboard',
      icon: <DashboardOutlined />,
      label: '生产看板',
    },
    {
      key: '/device-monitor',
      icon: <MonitorOutlined />,
      label: '设备监控',
    },
    {
      key: '/products',
      icon: <AppstoreOutlined />,
      label: '产品管理',
      roles: ['Admin', 'Technician'],
    },
    {
      key: '/workstations',
      icon: <EnvironmentOutlined />,
      label: '工位管理',
      roles: ['Admin', 'Technician'],
    },
    {
      key: '/workorders',
      icon: <FileTextOutlined />,
      label: '工单管理',
      roles: ['Admin', 'Planner'],
    },
    {
      key: '/workreports',
      icon: <FormOutlined />,
      label: '报工管理',
      roles: ['Admin', 'Operator'],
    },
    {
      key: '/users',
      icon: <TeamOutlined />,
      label: '用户管理',
      roles: ['Admin'],
    },
  ];

  // 根据用户角色过滤菜单
  const filteredMenuItems = menuItems.filter(item => {
    if (!item.roles) return true; // 没有角色限制的菜单项对所有人可见
    return item.roles.includes(userInfo?.roleName || '');
  });

  const userMenuItems = [
    {
      key: 'logout',
      icon: <LogoutOutlined />,
      label: '退出登录',
      onClick: handleLogout,
    },
  ];

  return (
    <AntLayout style={{ minHeight: '100vh' }}>
      <Header style={{
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'space-between',
        background: '#001529',
        padding: '0 24px'
      }}>
        <div style={{ color: 'white', fontSize: 20, fontWeight: 'bold' }}>
          Mini MES 系统
        </div>
        <Dropdown menu={{ items: userMenuItems }} placement="bottomRight">
          <div style={{ cursor: 'pointer', display: 'flex', alignItems: 'center', gap: 8 }}>
            <Avatar icon={<UserOutlined />} />
            <span style={{ color: 'white' }}>
              {userInfo?.realName} ({userInfo?.roleName})
            </span>
          </div>
        </Dropdown>
      </Header>
      <AntLayout>
        <Sider width={200} style={{ background: '#fff' }}>
          <Menu
            mode="inline"
            selectedKeys={[location.pathname]}
            style={{ height: '100%', borderRight: 0 }}
            items={filteredMenuItems}
            onClick={({ key }) => navigate(key)}
          />
        </Sider>
        <AntLayout style={{ padding: '24px' }}>
          <Content
            style={{
              background: '#fff',
              padding: 24,
              margin: 0,
              minHeight: 280,
            }}
          >
            <Outlet />
          </Content>
        </AntLayout>
      </AntLayout>
    </AntLayout>
  );
};

export default Layout;
