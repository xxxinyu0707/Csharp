using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // 需要引入以使用 DatabaseGenerated

namespace OrderManagementLogic
{
    // 订单明细实体类
    public class OrderDetails
    {
        [Key] // 主键
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 数据库自动生成ID
        public int OrderDetailId { get; set; }

        public string ProductName { get; set; } // 商品名称
        public int Quantity { get; set; }       // 数量
        public decimal Price { get; set; }        // 价格

        // 外键属性，指向所属订单
        public int OrderId { get; set; }

        // 导航属性，反向引用所属的订单对象 (virtual 关键字允许延迟加载)
        public virtual Order Order { get; set; }

        // 基于主键的 Equals 和 GetHashCode 通常更适合EF
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            OrderDetails other = (OrderDetails)obj;
            // 如果ID都已生成（非0），则基于ID比较
            if (this.OrderDetailId != 0 && other.OrderDetailId != 0)
            {
                return this.OrderDetailId == other.OrderDetailId;
            }
            // 对于未保存或需要内容比较的情况
            return ProductName == other.ProductName &&
                  Quantity == other.Quantity &&
                  Price == other.Price &&
                  OrderId == other.OrderId;
        }

        public override int GetHashCode()
        {
            if (OrderDetailId != 0) return OrderDetailId.GetHashCode(); // 已保存实体用ID

            // 未保存实体基于内容计算哈希码
            int hash = 17;
            hash = hash * 23 + (ProductName?.GetHashCode() ?? 0);
            hash = hash * 23 + Quantity.GetHashCode();
            hash = hash * 23 + Price.GetHashCode();
            hash = hash * 23 + OrderId.GetHashCode();
            return hash;
        }

        // 用于显示的 ToString 方法
        public override string ToString()
        {
            // 使用 C2 格式化货币
            return $"  商品名称: {ProductName}, 数量: {Quantity}, 价格: {Price:C2}";
        }
    }
}
