using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OrderManagementLogic; // 引入逻辑层命名空间

namespace OrderManagementUI
{
    public partial class Form1 : Form
    {
        private OrderService orderService; // 订单服务实例

        public Form1()
        {
            InitializeComponent();
            orderService = new OrderService(); // 创建服务实例

            // 初始化查询下拉框
            cmbSearchType.Items.AddRange(new string[] { "所有订单", "按订单号", "按客户名" });
            cmbSearchType.SelectedIndex = 0;

            // 设置数据绑定源和成员 (在设计器中设置更佳)
            bsOrderDetails.DataSource = bsOrders;
            bsOrderDetails.DataMember = "OrderDetails";
            // dgvOrders.DataSource = bsOrders; (设计器设置)
            // dgvOrderDetails.DataSource = bsOrderDetails; (设计器设置)

            // 初始加载数据
            LoadOrders();

            // 绑定事件，当选中订单改变时更新按钮状态
            bsOrders.CurrentChanged += (s, e) => UpdateButtonStates();
            UpdateButtonStates(); // 初始化按钮状态
        }

        // 加载或刷新订单列表的方法
        private void LoadOrders(List<Order> orders = null)
        {
            try
            {
                // 如果未提供特定列表，则从服务获取所有订单（默认按ID排序）
                bsOrders.DataSource = orders ?? orderService.SortOrdersByOrderId();
                bsOrders.ResetBindings(false); // 通知绑定控件数据已更改
            }
            catch (Exception ex)
            {
                // 捕获数据库连接等错误
                MessageBox.Show($"加载订单时出错: {ex.Message}\n请检查数据库连接和配置。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bsOrders.DataSource = new List<Order>(); // 出错时显示空列表
                bsOrders.ResetBindings(false);
            }
            finally // 无论成功或失败，都更新按钮状态
            {
                UpdateButtonStates();
            }
        }

        // 更新编辑和删除按钮的启用状态
        private void UpdateButtonStates()
        {
            bool orderSelected = bsOrders.Current != null;
            btnEdit.Enabled = orderSelected;
            btnDelete.Enabled = orderSelected;
        }

        // 查询按钮点击事件
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string criteria = cmbSearchType.SelectedItem.ToString();
                string value = txtSearchValue.Text.Trim();
                List<Order> results = null;

                switch (criteria)
                {
                    case "按订单号":
                        if (int.TryParse(value, out int id))
                        {
                            results = orderService.QueryOrdersByOrderId(id);
                        }
                        else
                        {
                            MessageBox.Show("请输入有效的数字订单号。");
                            results = new List<Order>(); // 显示空列表
                        }
                        break;
                    case "按客户名":
                        // QueryOrdersByCustomerName 内部处理空字符串情况
                        results = orderService.QueryOrdersByCustomerName(value);
                        break;
                    case "所有订单":
                    default:
                        results = orderService.SortOrdersByOrderId(); // 默认获取所有并按ID排序
                        break;
                }
                LoadOrders(results); // 使用查询结果加载列表
            }
            catch (Exception ex)
            {
                MessageBox.Show($"查询订单时出错: {ex.Message}", "查询错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 添加按钮点击事件
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 创建一个临时的空订单对象传递给编辑窗口
            Order newOrder = new Order();
            OrderEditForm editForm = new OrderEditForm(newOrder, true, orderService); // isNew = true

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadOrders(); // 添加成功后刷新主列表
                              // (可选) 找到新添加的订单并选中它
                var addedOrder = bsOrders.List.OfType<Order>().FirstOrDefault(o => o.OrderId == newOrder.OrderId);
                if (addedOrder != null)
                {
                    bsOrders.Position = bsOrders.IndexOf(addedOrder);
                }
            }
        }

        // 编辑按钮点击事件
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (bsOrders.Current is Order selectedOrder)
            {
                // 记录当前选中的位置
                int currentPosition = bsOrders.Position;

                // 直接传递选中的订单对象给编辑窗口
                OrderEditForm editForm = new OrderEditForm(selectedOrder, false, orderService); // isNew = false

                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // 修改成功后刷新列表
                    LoadOrders();
                    // 尝试恢复原来的选中位置
                    if (currentPosition >= 0 && currentPosition < bsOrders.List.Count)
                    {
                        // 检查 ID 是否还存在于新列表中（以防万一被删除了）
                        if (bsOrders.List.OfType<Order>().Any(o => o.OrderId == selectedOrder.OrderId))
                        {
                            // 重新查找对象在新列表中的索引可能更可靠
                            var updatedOrder = bsOrders.List.OfType<Order>().FirstOrDefault(o => o.OrderId == selectedOrder.OrderId);
                            if (updatedOrder != null)
                                bsOrders.Position = bsOrders.IndexOf(updatedOrder);
                            else
                                bsOrders.Position = currentPosition; // 回退到按位置恢复
                        }
                        else
                        {
                            bsOrders.Position = Math.Min(currentPosition, bsOrders.List.Count - 1); // 如果原项没了，选最后一个或第一个
                        }
                    }
                }
                // else // 如果用户取消了编辑，不需要特别操作，数据未提交到数据库
            }
        }

        // 删除按钮点击事件
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (bsOrders.Current is Order selectedOrder)
            {
                if (MessageBox.Show($"确定要删除订单 {selectedOrder.OrderId} 吗？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        orderService.RemoveOrder(selectedOrder.OrderId);
                        LoadOrders(); // 删除成功后刷新列表
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"删除订单时出错: {ex.Message}", "删除错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // (可选) 双击订单列表行进行编辑
        private void dgvOrders_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // 确保双击的是有效行而不是标题行
            if (e.RowIndex >= 0)
            {
                btnEdit_Click(sender, e); // 调用编辑按钮的逻辑
            }
        }
    }
}