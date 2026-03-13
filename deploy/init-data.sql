-- ============================================
-- Mini MES 系统初始化数据脚本
-- ============================================

-- 1. 清空现有数据（可选，谨慎使用）
-- TRUNCATE TABLE work_reports;
-- TRUNCATE TABLE work_orders;
-- TRUNCATE TABLE workstations;
-- TRUNCATE TABLE products;
-- TRUNCATE TABLE users;
-- TRUNCATE TABLE roles;

-- 2. 插入角色数据
INSERT INTO roles (role_name, permissions, created_at, updated_at) VALUES
('Admin', '["*"]', UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('Planner', '["workorder:read","workorder:write"]', UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('Technician', '["product:read","product:write","workstation:read","workstation:write"]', UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('Operator', '["workreport:read","workreport:write"]', UTC_TIMESTAMP(), UTC_TIMESTAMP());

-- 3. 插入用户数据
-- 密码都是经过BCrypt加密的，原始密码：admin123 或 123456
INSERT INTO users (username, password_hash, real_name, role_id, status, created_at, updated_at) VALUES
('admin', '$2a$11$8vJ5YZxKqGqJ5YZxKqGqJeO5YZxKqGqJ5YZxKqGqJ5YZxKqGqJ5YZ', '系统管理员', 1, 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('planner01', '$2a$11$8vJ5YZxKqGqJ5YZxKqGqJeO5YZxKqGqJ5YZxKqGqJ5YZxKqGqJ5YZ', '计划员-张三', 2, 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('tech01', '$2a$11$8vJ5YZxKqGqJ5YZxKqGqJeO5YZxKqGqJ5YZxKqGqJ5YZxKqGqJ5YZ', '技术员-李四', 3, 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('operator01', '$2a$11$8vJ5YZxKqGqJ5YZxKqGqJeO5YZxKqGqJ5YZxKqGqJ5YZxKqGqJ5YZ', '操作工-王五', 4, 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('operator02', '$2a$11$8vJ5YZxKqGqJ5YZxKqGqJeO5YZxKqGqJ5YZxKqGqJ5YZxKqGqJ5YZ', '操作工-赵六', 4, 1, UTC_TIMESTAMP(), UTC_TIMESTAMP());

-- 4. 插入产品数据
INSERT INTO products (product_code, product_name, specification, status, created_at, updated_at) VALUES
('P001', '智能手机主板', '型号: MB-X1, 尺寸: 150x75mm', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('P002', '平板电脑屏幕', '10.1英寸 IPS屏幕, 分辨率: 1920x1200', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('P003', '笔记本电池', '容量: 5000mAh, 电压: 11.4V', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('P004', '无线耳机外壳', '材质: ABS塑料, 颜色: 黑色', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('P005', '智能手表表带', '材质: 硅胶, 长度: 22cm', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP());

-- 5. 插入工位数据
INSERT INTO workstations (station_code, station_name, workshop, status, created_at, updated_at) VALUES
('WS001', 'SMT贴片工位', '一车间', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('WS002', '组装工位A', '一车间', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('WS003', '测试工位', '二车间', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('WS004', '包装工位', '二车间', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('WS005', '质检工位', '三车间', 1, UTC_TIMESTAMP(), UTC_TIMESTAMP());

-- 6. 插入工单数据
INSERT INTO work_orders (order_no, product_id, target_quantity, completed_quantity, defect_quantity, status, created_by, started_at, completed_at, created_at, updated_at) VALUES
('WO20260313001', 1, 1000, 650, 50, 1, 1, DATE_SUB(UTC_TIMESTAMP(), INTERVAL 2 DAY), NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('WO20260313002', 2, 500, 500, 20, 2, 1, DATE_SUB(UTC_TIMESTAMP(), INTERVAL 5 DAY), DATE_SUB(UTC_TIMESTAMP(), INTERVAL 1 DAY), UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('WO20260313003', 3, 800, 0, 0, 0, 1, NULL, NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('WO20260313004', 4, 1200, 300, 15, 1, 1, DATE_SUB(UTC_TIMESTAMP(), INTERVAL 1 DAY), NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP()),
('WO20260313005', 5, 600, 0, 0, 0, 1, NULL, NULL, UTC_TIMESTAMP(), UTC_TIMESTAMP());

-- 7. 插入报工记录数据
INSERT INTO work_reports (work_order_id, workstation_id, good_quantity, defect_quantity, reported_by, reported_at, remark, created_at, updated_at) VALUES
-- 工单1的报工记录
(1, 1, 400, 30, 1, DATE_SUB(UTC_TIMESTAMP(), INTERVAL 2 DAY), '首批生产', UTC_TIMESTAMP(), UTC_TIMESTAMP()),
(1, 2, 250, 20, 1, DATE_SUB(UTC_TIMESTAMP(), INTERVAL 1 DAY), '第二批生产', UTC_TIMESTAMP(), UTC_TIMESTAMP()),
-- 工单2的报工记录（已完工）
(2, 1, 300, 10, 1, DATE_SUB(UTC_TIMESTAMP(), INTERVAL 5 DAY), '第一批', UTC_TIMESTAMP(), UTC_TIMESTAMP()),
(2, 3, 200, 10, 1, DATE_SUB(UTC_TIMESTAMP(), INTERVAL 3 DAY), '第二批', UTC_TIMESTAMP(), UTC_TIMESTAMP()),
-- 工单4的报工记录
(4, 2, 300, 15, 1, DATE_SUB(UTC_TIMESTAMP(), INTERVAL 1 DAY), '初始生产批次', UTC_TIMESTAMP(), UTC_TIMESTAMP());

-- ============================================
-- 验证数据
-- ============================================
SELECT '角色数据' AS '表名', COUNT(*) AS '记录数' FROM roles
UNION ALL
SELECT '用户数据', COUNT(*) FROM users
UNION ALL
SELECT '产品数据', COUNT(*) FROM products
UNION ALL
SELECT '工位数据', COUNT(*) FROM workstations
UNION ALL
SELECT '工单数据', COUNT(*) FROM work_orders
UNION ALL
SELECT '报工记录', COUNT(*) FROM work_reports;

-- ============================================
-- 注意事项
-- ============================================
-- 1. 用户密码需要使用实际的BCrypt哈希值
--    当前脚本中的password_hash是占位符，需要替换为真实的哈希值
--    可以通过后端API创建用户，或使用BCrypt工具生成哈希值
--
-- 2. 默认密码：
--    - admin / admin123
--    - 其他用户 / 自定义
--
-- 3. 如果需要重新初始化，先执行TRUNCATE语句清空数据
--    注意：TRUNCATE会删除所有数据且不可恢复
--
-- 4. 工单和报工记录的ID关联需要根据实际插入后的ID调整
--    或者使用子查询动态获取ID
