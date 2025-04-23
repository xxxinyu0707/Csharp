using Microsoft.EntityFrameworkCore; // 引入 EF Core 命名空间
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderManagementLogic
{
    // 订单服务类，处理业务逻辑并与数据库交互
    public class OrderService
    {
        // 添加新订单
        public void AddOrder(Order order)
        {
            // 使用 using 确保 DbContext 被正确释放
            using (var context = new OrderContext())
            {
                // 检查订单号是否已存在 (因为用户可以输入)
                if (context.Orders.Any(o => o.OrderId == order.OrderId))
                {
                    throw new Exception($"订单号 {order.OrderId} 已存在！");
                }

                // 添加订单到上下文，EF Core 会自动跟踪订单及其包含的明细
                context.Orders.Add(order);
                // 保存更改到数据库
                context.SaveChanges();
            }
        }

        // 根据 ID 删除订单
        public void RemoveOrder(int orderId)
        {
            using (var context = new OrderContext())
            {
                // 查找要删除的订单
                // 由于配置了级联删除，通常不需要 Include 明细来删除
                var orderToRemove = context.Orders.FirstOrDefault(o => o.OrderId == orderId);

                if (orderToRemove == null)
                {
                    throw new Exception("订单不存在！");
                }

                // 从上下文中移除订单，EF Core 会处理级联删除
                context.Orders.Remove(orderToRemove);
                // 保存更改
                context.SaveChanges();
            }
        }

        // 更新现有订单
        public void UpdateOrder(Order updatedOrder)
        {
            using (var context = new OrderContext())
            {
                // 查找数据库中的现有订单，并加载其当前的明细
                var existingOrder = context.Orders
                                           .Include(o => o.OrderDetails) // 加载现有明细用于后续操作
                                           .FirstOrDefault(o => o.OrderId == updatedOrder.OrderId);

                if (existingOrder == null)
                {
                    throw new Exception("要修改的订单不存在！");
                }

                // 更新订单的基本信息 (非关联数据)
                existingOrder.CustomerName = updatedOrder.CustomerName;

                // --- 更新订单明细 (采用简单策略：先删除旧的，再添加新的) ---

                // 1. 从上下文中移除与此订单关联的所有旧明细
                context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

                // 2. 添加来自 'updatedOrder' 的新明细
                foreach (var detail in updatedOrder.OrderDetails)
                {
                    detail.OrderId = existingOrder.OrderId; // 确保外键设置正确
                    detail.OrderDetailId = 0; // 重置主键，让数据库生成新的
                    // 不需要设置 detail.Order 导航属性

                    // 将新明细添加到上下文进行跟踪
                    context.OrderDetails.Add(detail);
                }

                // 保存所有更改 (客户名修改、旧明细删除、新明细添加)
                context.SaveChanges();
            }
        }

        // 获取所有订单 (包含明细)
        public List<Order> GetAllOrders()
        {
            using (var context = new OrderContext())
            {
                // 使用 Include 预加载订单明细，避免 N+1 查询问题
                return context.Orders.Include(o => o.OrderDetails).ToList();
            }
        }

        // 按订单号查询 (包含明细)
        public List<Order> QueryOrdersByOrderId(int orderId)
        {
            using (var context = new OrderContext())
            {
                return context.Orders
                              .Include(o => o.OrderDetails)
                              .Where(o => o.OrderId == orderId)
                              .ToList();
            }
        }

        // 按客户名查询 (包含明细，模糊查询，不区分大小写)
        public List<Order> QueryOrdersByCustomerName(string customerName)
        {
            using (var context = new OrderContext())
            {
                if (string.IsNullOrWhiteSpace(customerName))
                {
                    // 如果查询条件为空，返回所有订单
                    return GetAllOrders();
                }
                // 转换为小写进行不区分大小写的包含查询
                return context.Orders
                             .Include(o => o.OrderDetails)
                             .Where(o => o.CustomerName.ToLower().Contains(customerName.ToLower()))
                             .ToList();
            }
        }

        // 按总金额排序 (需加载到内存后排序)
        public List<Order> GetAllOrdersSortedByTotalAmount()
        {
            using (var context = new OrderContext())
            {
                // 加载所有订单及明细到内存
                var allOrders = context.Orders.Include(o => o.OrderDetails).ToList();
                // 在内存中根据计算属性 TotalAmount 排序
                return allOrders.OrderBy(o => o.TotalAmount).ToList();
            }
        }

        // 按订单号排序 (数据库可高效处理)
        public List<Order> SortOrdersByOrderId()
        {
            using (var context = new OrderContext())
            {
                return context.Orders
                              .Include(o => o.OrderDetails)
                              .OrderBy(o => o.OrderId)
                              .ToList();
            }
        }

        // 通用排序方法 (在内存中排序较简单)
        public List<Order> SortOrders(Func<Order, object> keySelector)
        {
            using (var context = new OrderContext())
            {
                // 加载到内存再排序
                var allOrders = context.Orders.Include(o => o.OrderDetails).ToList();
                return allOrders.OrderBy(keySelector).ToList();
            }
        }
    }
}