import { createBrowserRouter, Navigate } from 'react-router-dom';
import Login from '../pages/Login';
import Layout from '../components/Layout';
import Dashboard from '../pages/Dashboard';
import ProductList from '../pages/Product/List';
import WorkstationList from '../pages/Workstation/List';
import WorkOrderList from '../pages/WorkOrder/List';
import WorkReportList from '../pages/WorkReport/List';
import UserList from '../pages/User/List';
import DeviceMonitor from '../pages/DeviceMonitor';

const router = createBrowserRouter([
  {
    path: '/login',
    element: <Login />,
  },
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        index: true,
        element: <Navigate to="/dashboard" replace />,
      },
      {
        path: 'dashboard',
        element: <Dashboard />,
      },
      {
        path: 'device-monitor',
        element: <DeviceMonitor />,
      },
      {
        path: 'products',
        element: <ProductList />,
      },
      {
        path: 'workstations',
        element: <WorkstationList />,
      },
      {
        path: 'workorders',
        element: <WorkOrderList />,
      },
      {
        path: 'workreports',
        element: <WorkReportList />,
      },
      {
        path: 'users',
        element: <UserList />,
      },
    ],
  },
]);

export default router;
