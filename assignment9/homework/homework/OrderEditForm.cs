using System;
using System.Linq;
using System.Windows.Forms;
using OrderManagementLogic; // 引入逻辑层命名空间

namespace OrderManagementUI
{
    public partial class OrderEditForm : Form
    {
        public Order CurrentOrder { get; private set; } // 当前正在编辑或新建的订单
        private bool isNew;                             // 标记是新建还是编辑
        private OrderService orderService;              // 用于调用添加或更新方法

        // 构造函数
        public OrderEditForm(Order order, bool isNewOrder, OrderService service)
        {
            InitializeComponent();

            CurrentOrder = order;
            isNew = isNewOrder;
            orderService = service;

            // 根据是新建还是修改，设置窗口标题和订单号控件状态
            this.Text = isNew ? "添加新订单" : $"修改订单 {CurrentOrder.OrderId}";
            txtOrderId.ReadOnly = !isNew; // 只有新建时才允许输入订单号

            // 绑定数据到控件
            BindData();

            // (可选) 处理 DataGridView 数据输入错误
            dgvEditDetails.DataError += DgvEditDetails_DataError;
        }

        // 将订单数据绑定到窗口控件
        private void BindData()
        {
            // 清除可能存在的旧绑定
            txtOrderId.DataBindings.Clear();
            txtCustomerName.DataBindings.Clear();
            bsEditDetails.DataSource = null; // 先解绑

            // 绑定订单号和客户名
            if (isNew)
            {
                // 新建时，订单号文本框允许输入，不进行数据绑定
                // CustomerName 绑定，以便输入时更新对象
                txtCustomerName.DataBindings.Add("Text", CurrentOrder, "CustomerName", false, DataSourceUpdateMode.OnPropertyChanged);
            }
            else
            {
                // 修改时，显示订单号，并绑定客户名
                txtOrderId.Text = CurrentOrder.OrderId.ToString(); // 直接显示ID
                txtCustomerName.DataBindings.Add("Text", CurrentOrder, "CustomerName", false, DataSourceUpdateMode.OnPropertyChanged);
            }


            // 绑定订单明细 DataGridView
            // 确保 CurrentOrder.OrderDetails 列表已初始化
            if (CurrentOrder.OrderDetails == null)
            {
                CurrentOrder.OrderDetails = new System.Collections.Generic.List<OrderDetails>();
            }
            bsEditDetails.DataSource = CurrentOrder.OrderDetails;
            // dgvEditDetails.DataSource = bsEditDetails; // 在设计器中设置
            bsEditDetails.ResetBindings(false); // 刷新绑定
        }

        // 保存按钮点击事件
        private void btnSave_Click(object sender, EventArgs e)
        {
            // 结束编辑状态，确保 DataGridView 数据已写入 BindingSource
            this.ValidateChildren(); // 触发表单验证
            dgvEditDetails.EndEdit();
            bsEditDetails.EndEdit(); // 提交 BindingSource 的更改

            // --- 执行验证 ---
            int orderId = 0;
            if (isNew) // 仅在新建时验证和读取订单号
            {
                if (!int.TryParse(txtOrderId.Text, out orderId) || orderId <= 0)
                {
                    MessageBox.Show("请输入一个有效的正整数订单号。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtOrderId.Focus();
                    this.DialogResult = DialogResult.None; // 阻止窗口关闭
                    return;
                }
                CurrentOrder.OrderId = orderId; // 将验证后的ID赋给订单对象
                                                // 不需要在这里检查ID是否重复，AddOrder服务方法会检查
            }

            // 验证客户名 (已通过数据绑定更新到 CurrentOrder.CustomerName)
            if (string.IsNullOrWhiteSpace(CurrentOrder.CustomerName))
            {
                MessageBox.Show("客户名称不能为空。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            // 验证订单明细 (简单检查)
            foreach (var detail in CurrentOrder.OrderDetails)
            {
                // 忽略 DataGridView 自动添加的、可能未填写的最后一行空行
                if (detail == bsEditDetails.Current && string.IsNullOrWhiteSpace(detail.ProductName) && detail.Quantity == 0 && detail.Price == 0)
                    continue;


                if (string.IsNullOrWhiteSpace(detail.ProductName))
                {
                    MessageBox.Show("订单明细中的商品名称不能为空。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SelectGridRow(detail); // (可选) 选中错误行
                    this.DialogResult = DialogResult.None; return;
                }
                if (detail.Quantity <= 0)
                {
                    MessageBox.Show($"商品 '{detail.ProductName}' 的数量必须大于0。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SelectGridRow(detail);
                    this.DialogResult = DialogResult.None; return;
                }
                if (detail.Price < 0)
                {
                    MessageBox.Show($"商品 '{detail.ProductName}' 的价格不能为负数。", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SelectGridRow(detail);
                    this.DialogResult = DialogResult.None; return;
                }
            }

            // (可选) 移除 DataGridView 中完全为空的行（通常是最后一行新行）
            var emptyDetails = CurrentOrder.OrderDetails
                .Where(d => string.IsNullOrWhiteSpace(d.ProductName) && d.Quantity == 0 && d.Price == 0m)
                .ToList(); // 先转为列表再移除
            if (emptyDetails.Any())
            {
                foreach (var item in emptyDetails)
                {
                    bsEditDetails.Remove(item); // 从 BindingSource 移除会更新UI和底层集合
                }
            }

            // --- 调用 OrderService 执行数据库操作 ---
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

                // 如果操作成功，DialogResult 会是 OK (由按钮属性设置)，窗口将自动关闭
                this.DialogResult = DialogResult.OK; // 明确设置一下也可以
            }
            catch (Exception ex) // 捕获服务层抛出的异常 (如订单号重复、数据库错误等)
            {
                MessageBox.Show($"保存订单失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None; // 保存失败，阻止窗口关闭
            }
        }

        // 取消按钮事件 (通常不需要代码，按钮的 DialogResult 属性设为 Cancel 即可)
        // private void btnCancel_Click(object sender, EventArgs e) { }

        // (可选) 处理 DataGridView 数据输入格式错误
        private void DgvEditDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show($"输入错误，行 {e.RowIndex + 1} 列 '{dgvEditDetails.Columns[e.ColumnIndex].HeaderText}':\n{e.Exception.Message}\n请输入有效的数据。",
                            "数据格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            // e.ThrowException = false; // 阻止抛出异常，让用户可以继续编辑
            // e.Cancel = true; // 可以选择是否取消编辑操作，通常设为 false 或不设置，让用户修改
        }

        // (辅助方法，可选) 选中包含特定明细对象的行
        private void SelectGridRow(OrderDetails detail)
        {
            dgvEditDetails.ClearSelection();
            foreach (DataGridViewRow row in dgvEditDetails.Rows)
            {
                if (row.DataBoundItem == detail)
                {
                    row.Selected = true;
                    dgvEditDetails.CurrentCell = row.Cells[0]; // 将焦点移到该行第一个单元格
                    break;
                }
            }
        }
    }
}