using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homework
{
    public class OrderDetails
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            OrderDetails other = (OrderDetails)obj;
            return ProductName == other.ProductName &&
                   Quantity == other.Quantity &&
                   Price == other.Price;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (ProductName?.GetHashCode() ?? 0);
            hash = hash * 23 + Quantity.GetHashCode();
            hash = hash * 23 + Price.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"  商品名称: {ProductName}, 数量: {Quantity}, 价格: {Price}";
        }
    }

    // 订单类
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

        public decimal TotalAmount
        {
            get { return OrderDetails.Sum(d => d.Quantity * d.Price); }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Order other = (Order)obj;
            return OrderId == other.OrderId; // 订单号唯一
        }

        public override int GetHashCode()
        {
            return OrderId.GetHashCode();
        }

        public override string ToString()
        {
            string details = string.Join(Environment.NewLine, OrderDetails.Select(d => d.ToString()));
            return $"订单号: {OrderId}, 客户: {CustomerName}, 总金额: {TotalAmount}{Environment.NewLine}订单明细:{Environment.NewLine}{details}";
        }
    }

    // 订单服务类
    public class OrderService
    {
        private List<Order> orders = new List<Order>();

        public void AddOrder(Order order)
        {
            if (orders.Contains(order))
            {
                throw new Exception("订单已存在！");
            }
            orders.Add(order);
        }

        public void RemoveOrder(int orderId)
        {
            Order orderToRemove = orders.FirstOrDefault(o => o.OrderId == orderId);
            if (orderToRemove == null)
            {
                throw new Exception("订单不存在！");
            }
            orders.Remove(orderToRemove);
        }

        public void UpdateOrder(Order updatedOrder)
        {
            Order existingOrder = orders.FirstOrDefault(o => o.OrderId == updatedOrder.OrderId);
            if (existingOrder == null)
            {
                throw new Exception("订单不存在！");
            }

            existingOrder.CustomerName = updatedOrder.CustomerName;
            existingOrder.OrderDetails = updatedOrder.OrderDetails;
        }

        public List<Order> GetAllOrders()
        {
            return orders;
        }

        public List<Order> QueryOrdersByOrderId(int orderId)
        {
            return orders.Where(o => o.OrderId == orderId).ToList();
        }

        public List<Order> QueryOrdersByCustomerName(string customerName)
        {
            return orders.Where(o => o.CustomerName.Contains(customerName)).ToList();
        }

        public List<Order> GetAllOrdersSortedByTotalAmount()
        {
            return orders.OrderBy(o => o.TotalAmount).ToList();
        }

        public List<Order> SortOrdersByOrderId()
        {
            return orders.OrderBy(o => o.OrderId).ToList();
        }

        public List<Order> SortOrders(Func<Order, object> keySelector)
        {
            return orders.OrderBy(keySelector).ToList();
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            OrderService orderService = new OrderService();

            while (true)
            {
                Console.WriteLine("\n请选择操作：");
                Console.WriteLine("1. 添加订单");
                Console.WriteLine("2. 删除订单");
                Console.WriteLine("3. 修改订单");
                Console.WriteLine("4. 查询订单");
                Console.WriteLine("5. 显示所有订单");
                Console.WriteLine("6. 按照总金额排序显示订单");
                Console.WriteLine("0. 退出");

                Console.Write("请输入操作序号：");
                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddOrder(orderService);
                            break;
                        case "2":
                            RemoveOrder(orderService);
                            break;
                        case "3":
                            UpdateOrder(orderService);
                            break;
                        case "4":
                            QueryOrder(orderService);
                            break;
                        case "5":
                            ShowAllOrders(orderService);
                            break;
                        case "6":
                            ShowAllOrdersSortedByTotalAmount(orderService);
                            break;
                        case "0":
                            Console.WriteLine("程序结束。");
                            return;
                        default:
                            Console.WriteLine("无效的输入，请重新输入。");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"发生错误: {e.Message}");
                }
            }
        }

        // 添加订单
        static void AddOrder(OrderService orderService)
        {
            Console.Write("请输入订单号：");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("无效的订单号！");
                return;
            }

            Console.Write("请输入客户名称：");
            string customerName = Console.ReadLine();

            Order order = new Order { OrderId = orderId, CustomerName = customerName };

            while (true)
            {
                Console.WriteLine("\n请输入订单明细（输入 'done' 完成）：");
                Console.Write("商品名称：");
                string productName = Console.ReadLine();

                if (productName.ToLower() == "done")
                {
                    break;
                }

                Console.Write("数量：");
                if (!int.TryParse(Console.ReadLine(), out int quantity))
                {
                    Console.WriteLine("无效的数量！");
                    continue;
                }

                Console.Write("价格：");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    Console.WriteLine("无效的价格！");
                    continue;
                }

                order.OrderDetails.Add(new OrderDetails { ProductName = productName, Quantity = quantity, Price = price });
            }

            orderService.AddOrder(order);
            Console.WriteLine("订单添加成功！");
        }

        // 删除订单
        static void RemoveOrder(OrderService orderService)
        {
            Console.Write("请输入要删除的订单号：");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("无效的订单号！");
                return;
            }

            orderService.RemoveOrder(orderId);
            Console.WriteLine("订单删除成功！");
        }

        // 修改订单
        static void UpdateOrder(OrderService orderService)
        {
            Console.Write("请输入要修改的订单号：");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("无效的订单号！");
                return;
            }

            Order existingOrder = orderService.GetAllOrders().FirstOrDefault(o => o.OrderId == orderId);
            if (existingOrder == null)
            {
                Console.WriteLine("订单不存在！");
                return;
            }

            Console.Write("请输入新的客户名称：");
            string customerName = Console.ReadLine();

            existingOrder.CustomerName = customerName;
            existingOrder.OrderDetails.Clear(); // 先清空原有的订单明细

            while (true)
            {
                Console.WriteLine("\n请输入新的订单明细（输入 'done' 完成）：");
                Console.Write("商品名称：");
                string productName = Console.ReadLine();

                if (productName.ToLower() == "done")
                {
                    break;
                }

                Console.Write("数量：");
                if (!int.TryParse(Console.ReadLine(), out int quantity))
                {
                    Console.WriteLine("无效的数量！");
                    continue;
                }

                Console.Write("价格：");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    Console.WriteLine("无效的价格！");
                    continue;
                }

                existingOrder.OrderDetails.Add(new OrderDetails { ProductName = productName, Quantity = quantity, Price = price });
            }

            orderService.UpdateOrder(existingOrder);
            Console.WriteLine("订单修改成功！");
        }

        // 查询订单
        static void QueryOrder(OrderService orderService)
        {
            Console.WriteLine("请选择查询方式：");
            Console.WriteLine("1. 按照订单号查询");
            Console.WriteLine("2. 按照客户名称查询");
            Console.Write("请输入查询方式：");
            string queryChoice = Console.ReadLine();

            switch (queryChoice)
            {
                case "1":
                    Console.Write("请输入要查询的订单号：");
                    if (!int.TryParse(Console.ReadLine(), out int orderId))
                    {
                        Console.WriteLine("无效的订单号！");
                        return;
                    }

                    List<Order> ordersByOrderId = orderService.QueryOrdersByOrderId(orderId);
                    ShowOrders(ordersByOrderId);
                    break;
                case "2":
                    Console.Write("请输入要查询的客户名称：");
                    string customerName = Console.ReadLine();

                    List<Order> ordersByCustomerName = orderService.QueryOrdersByCustomerName(customerName);
                    ShowOrders(ordersByCustomerName);
                    break;
                default:
                    Console.WriteLine("无效的查询方式！");
                    break;
            }
        }

        // 显示所有订单
        static void ShowAllOrders(OrderService orderService)
        {
            List<Order> allOrders = orderService.GetAllOrders();
            ShowOrders(allOrders);
        }

        // 按照总金额排序显示订单
        static void ShowAllOrdersSortedByTotalAmount(OrderService orderService)
        {
            List<Order> sortedOrders = orderService.GetAllOrdersSortedByTotalAmount();
            ShowOrders(sortedOrders);
        }

        // 显示订单列表
        static void ShowOrders(List<Order> orders)
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("没有找到符合条件的订单！");
                return;
            }

            foreach (Order order in orders)
            {
                Console.WriteLine(order);
            }
        }
    }
}
