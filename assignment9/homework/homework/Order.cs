using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // 需要引入以使用 NotMapped 和 DatabaseGenerated
using System.Linq;

namespace OrderManagementLogic
{
    // 订单实体类
    public class Order
    {
        [Key] // 主键
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // ID不由数据库生成，用户输入
        public int OrderId { get; set; }

        public string CustomerName { get; set; } // 客户名称

        // 导航属性，包含该订单的所有明细 (virtual 关键字允许延迟加载)
        public virtual List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

        // 总金额是计算属性，不映射到数据库列
        [NotMapped]
        public decimal TotalAmount
        {
            // 确保 OrderDetails 列表不为 null
            get { return OrderDetails?.Sum(d => d.Quantity * d.Price) ?? 0m; }
        }

        // 基于主键 OrderId 的 Equals 和 GetHashCode
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Order other = (Order)obj;
            return OrderId == other.OrderId;
        }

        public override int GetHashCode()
        {
            return OrderId.GetHashCode();
        }

        // 用于显示的 ToString 方法
        public override string ToString()
        {
            // 检查 OrderDetails 是否为 null
            string details = OrderDetails != null ? string.Join(Environment.NewLine, OrderDetails.Select(d => d.ToString())) : "  (无明细)";
            // 使用 C2 格式化货币
            return $"订单号: {OrderId}, 客户: {CustomerName}, 总金额: {TotalAmount:C2}{Environment.NewLine}订单明细:{Environment.NewLine}{details}";
        }
    }
}
