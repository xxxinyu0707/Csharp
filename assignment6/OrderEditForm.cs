using System;
using System.Linq;
using System.Windows.Forms;
using homework;
using OrderManagementLogic;

namespace OrderManagementUI
{
    public partial class OrderEditForm : Form
    {
        public Order CurrentOrder { get; private set; }
        private bool isNew;
        private OrderService orderService;

        // 构造函数，接收订单对象、是否为新建、以及订单服务实例
        public OrderEditForm(Order order, bool isNewOrder, OrderService service)
        {
            InitializeComponent();

            CurrentOrder = order;
            isNew = isNewOrder;
            orderService = service;

            // 根据是新建还是修改来设置窗口标题和订单号的可编辑性
            this.Text = isNew ? "添加新订单" : $"修改订单 {CurrentOrder.OrderId}";
            txtOrderId.ReadOnly = !isNew; // 新建时才允许编辑订单号

            // 数据绑定
            BindData();
        }

        private void BindData()
        {
            // 清除旧的绑定（以防万一）
            txtOrderId.DataBindings.Clear();
            txtCustomerName.DataBindings.Clear();

            // 绑定订单号和客户名输入框到 CurrentOrder 对象
            // 对于新建，订单号需要手动读取；对于修改，显示即可
            if (!isNew)
            {
                txtOrderId.Text = CurrentOrder.OrderId.ToString(); // 修改时直接显示ID
            }
            // CustomerName 使用数据绑定，OnPropertyChanged 可以在输入时实时更新对象
            txtCustomerName.DataBindings.Add("Text", CurrentOrder, "CustomerName", false, DataSourceUpdateMode.OnPropertyChanged);

            // 绑定 DataGridView 的数据源到 BindingSource
            // 确保 CurrentOrder.OrderDetails 不是 null
            if (CurrentOrder.OrderDetails == null)
            {
                CurrentOrder.OrderDetails = new System.Collections.Generic.List<OrderDetails>();
            }
            bsEditDetails.DataSource = CurrentOrder.OrderDetails;
            // dgvEditDetails.DataSource = bsEditDetails; // 设计器已设置
            bsEditDetails.ResetBindings(false);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 强制结束 DataGridView 的编辑状态，确保数据写入 BindingSource
            dgvEditDetails.EndEdit();
            bsEditDetails.EndEdit(); // 提交 BindingSource 的更改到基础对象 (CurrentOrder.OrderDetails)


            // --- 基本验证 ---
            int orderId = 0;
            if (isNew)
            {
                if (!int.TryParse(txtOrderId.Text, out orderId) || orderId <= 0)
                {
                    MessageBox.Show("请输入一个有效的正整数订单号。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOrderId.Focus();
                    this.DialogResult = DialogResult.None; // 阻止窗口关闭
                    return;
                }
                // 检查ID是否已存在 (仅新建时需要)
                if (orderService.QueryOrdersByOrderId(orderId).Any())
                {
                    MessageBox.Show($"订单号 {orderId} 已存在。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOrderId.Focus();
                    this.DialogResult = DialogResult.None; // 阻止窗口关闭
                    return;
                }
                CurrentOrder.OrderId = orderId; // 将验证后的 ID 赋给对象
            }

            if (string.IsNullOrWhiteSpace(CurrentOrder.CustomerName)) // CustomerName 已通过绑定更新
            {
                MessageBox.Show("客户名称不能为空。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                this.DialogResult = DialogResult.None; // 阻止窗口关闭
                return;
            }

            // 验证订单明细 (简单示例：检查是否有商品名为空或数量价格无效)
            foreach (var detail in CurrentOrder.OrderDetails)
            {
                // 忽略 DataGridView 自动添加的、用户可能没填完的最后一行
                if (bsEditDetails.Current == detail && string.IsNullOrWhiteSpace(detail.ProductName) && detail.Quantity == 0) continue;

                if (string.IsNullOrWhiteSpace(detail.ProductName))
                {
                    MessageBox.Show("订单明细中的商品名称不能为空。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None; return;
                }
                if (detail.Quantity <= 0)
                {
                    MessageBox.Show($"商品 '{detail.ProductName}' 的数量必须大于0。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None; return;
                }
                if (detail.Price < 0)
                {
                    MessageBox.Show($"商品 '{detail.ProductName}' 的价格不能为负。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.DialogResult = DialogResult.None; return;
                }
            }

            // (可选) 移除 DataGridView 中完全为空的行（通常是最后一行新行）
            var emptyDetails = CurrentOrder.OrderDetails
                .Where(d => string.IsNullOrWhiteSpace(d.ProductName) && d.Quantity == 0 && d.Price == 0)
                .ToList();
            if (emptyDetails.Any())
            {
                foreach (var item in emptyDetails)
                {
                    bsEditDetails.Remove(item); // 从 BindingSource 移除
                }
            }


            // --- 调用服务执行操作 ---
            try
            {
                if (isNew)
                {
                    orderService.AddOrder(CurrentOrder);
                }
                else
                {
                    orderService.UpdateOrder(CurrentOrder);
                }

                // 成功后，DialogResult 会是 OK（设计器设置的），窗口自动关闭
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存订单失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // 保存失败，阻止窗口关闭
            }
        }

        // 取消按钮点击事件 (通常不需要代码，因为按钮的 DialogResult 属性已设为 Cancel)
        // private void btnCancel_Click(object sender, EventArgs e) { }

        // 处理 DataGridView 中的数据输入错误 (可选，简化版可以去掉)
        private void dgvEditDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"输入错误，行{e.RowIndex + 1} 列{e.ColumnIndex + 1}: {e.Exception.Message}", "数据错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.Cancel = true; // 阻止提交错误数据，让用户修改
        }
    }
}