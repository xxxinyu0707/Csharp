using Microsoft.AspNetCore.Mvc;
using OrderManagementLogic; // 引入逻辑层命名空间
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // 可能需要引入以处理 Include 等

namespace OrderManagementAPI.Controllers
{
    [ApiController] // 标记这是一个 API 控制器
    [Route("api/[controller]")] // 定义基础路由: /api/orders
    public class OrdersController : ControllerBase // 继承 ControllerBase
    {
        private readonly OrderService _orderService; // 注入 OrderService
        private readonly ILogger<OrdersController> _logger; // 可选：注入日志记录器

        // 构造函数，通过 DI 注入服务
        public OrdersController(OrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger; // 可选
        }

        // GET: api/orders
        // 获取所有订单
        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetOrders([FromQuery] string? customerName = null, [FromQuery] string? sortBy = null)
        {
            try
            {
                List<Order> orders;
                // --- 查询 ---
                if (!string.IsNullOrWhiteSpace(customerName))
                {
                    orders = _orderService.QueryOrdersByCustomerName(customerName);
                }
                else
                {
                    // 获取所有订单，可以基于 sortBy 参数决定初始排序
                    // 这里我们默认按 ID 排序，如果需要其他默认排序，修改 OrderService 或这里
                    orders = _orderService.GetAllOrders(); // GetAllOrders 内部可能已排序，或在此排序
                }

                // --- 排序 (在服务层处理更好，但这里也可以做简单补充) ---
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    switch (sortBy.ToLower())
                    {
                        case "id":
                            orders = orders.OrderBy(o => o.OrderId).ToList();
                            break;
                        case "customer":
                            orders = orders.OrderBy(o => o.CustomerName).ToList();
                            break;
                        case "totalamount":
                            // 注意：TotalAmount 是计算属性，排序可能在内存中进行
                            orders = orders.OrderBy(o => o.TotalAmount).ToList();
                            break;
                            // 可以添加降序等更多选项: "id_desc", "customer_desc" ...
                    }
                }


                return Ok(orders); // 返回 200 OK 和订单列表
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "获取订单列表时出错。"); // 可选日志
                return StatusCode(500, $"获取订单时发生内部错误: {ex.Message}"); // 返回 500 Internal Server Error
            }
        }

        // GET: api/orders/{id}
        // 根据 ID 获取单个订单
        [HttpGet("{id}")]
        public ActionResult<Order> GetOrder(int id)
        {
            try
            {
                // QueryOrdersByOrderId 返回 List<Order>，但我们通常期望 GET /id 返回单个对象或 404
                var order = _orderService.QueryOrdersByOrderId(id).FirstOrDefault(); // 获取第一个匹配项

                if (order == null)
                {
                    return NotFound($"未找到 ID 为 {id} 的订单。"); // 返回 404 Not Found
                }
                return Ok(order); // 返回 200 OK 和订单对象
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "获取 ID 为 {OrderId} 的订单时出错。", id);
                return StatusCode(500, $"获取订单 {id} 时发生内部错误: {ex.Message}");
            }
        }

        // POST: api/orders
        // 创建新订单
        // [FromBody] 指示 ASP.NET Core 从请求体中读取 Order 对象
        [HttpPost]
        public ActionResult<Order> CreateOrder([FromBody] Order order)
        {
            if (order == null || !ModelState.IsValid) // ModelState.IsValid 检查基于 DataAnnotations 的验证
            {
                // 如果模型验证失败（例如，如果添加了 [Required] 但未提供），返回 400
                return BadRequest(ModelState);
            }

            try
            {
                // 确保 OrderId 由客户端提供，因为 Order 实体配置了 DatabaseGeneratedOption.None
                if (order.OrderId <= 0)
                {
                    return BadRequest("创建订单时必须提供一个有效的正整数 OrderId。");
                }

                // 确保 OrderDetails 集合不是 null
                order.OrderDetails ??= new List<OrderDetails>();
                // (可选) 在添加前，确保明细的 OrderId 与主订单匹配或由服务层处理
                foreach (var detail in order.OrderDetails)
                {
                    // detail.OrderId = order.OrderId; // 服务层 UpdateOrder 会做类似的事，这里 AddOrder 逻辑可能也需要
                    detail.OrderDetailId = 0; // 确保明细ID由数据库生成
                }


                _orderService.AddOrder(order);

                // 返回 201 Created，并提供新资源的 URI 和对象本身
                // nameof(GetOrder) 是获取 GetOrder(int id) 这个 Action 方法的名字
                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
            }
            catch (Exception ex) // 捕获 OrderService 可能抛出的异常（如 ID 重复）
            {
                _logger?.LogError(ex, "创建订单时出错。订单数据: {@OrderData}", order);
                // 检查是否是特定错误，如 ID 重复
                if (ex.Message.Contains("已存在")) // 简单的检查
                {
                    return Conflict($"创建订单失败：订单号 {order.OrderId} 已存在。"); // 返回 409 Conflict
                }
                return StatusCode(500, $"创建订单时发生内部错误: {ex.Message}");
            }
        }

        // PUT: api/orders/{id}
        // 更新现有订单
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, [FromBody] Order updatedOrder)
        {
            if (updatedOrder == null || id != updatedOrder.OrderId)
            {
                // 如果 URL 中的 ID 和请求体中的 OrderId 不匹配，返回 400
                return BadRequest("URL 中的订单 ID 与请求体中的订单 ID 不匹配。");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 确保 OrderDetails 集合不是 null
                updatedOrder.OrderDetails ??= new List<OrderDetails>();
                // UpdateOrder 服务层会处理明细的更新逻辑 (移除旧的，添加新的)
                // 确保新添加的明细 ID 为 0，让数据库生成
                foreach (var detail in updatedOrder.OrderDetails)
                {
                    detail.OrderDetailId = 0; // 服务层代码已经做了这个
                }

                _orderService.UpdateOrder(updatedOrder);
                // 通常 PUT 成功后返回 204 No Content，表示资源已更新，但没有内容返回
                return NoContent();
                // 或者返回 200 OK 和更新后的对象： return Ok(updatedOrder);
            }
            catch (Exception ex) // 捕获 OrderService 可能抛出的异常（如订单不存在）
            {
                _logger?.LogError(ex, "更新 ID 为 {OrderId} 的订单时出错。更新数据: {@OrderData}", id, updatedOrder);
                if (ex.Message.Contains("不存在")) // 简单的检查
                {
                    return NotFound($"更新失败：未找到 ID 为 {id} 的订单。"); // 返回 404 Not Found
                }
                return StatusCode(500, $"更新订单 {id} 时发生内部错误: {ex.Message}");
            }
        }

        // DELETE: api/orders/{id}
        // 删除订单
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            try
            {
                _orderService.RemoveOrder(id);
                // 删除成功通常返回 204 No Content
                return NoContent();
            }
            catch (Exception ex) // 捕获 OrderService 可能抛出的异常（如订单不存在）
            {
                _logger?.LogError(ex, "删除 ID 为 {OrderId} 的订单时出错。", id);
                if (ex.Message.Contains("不存在")) // 简单的检查
                {
                    return NotFound($"删除失败：未找到 ID 为 {id} 的订单。"); // 返回 404 Not Found
                }
                return StatusCode(500, $"删除订单 {id} 时发生内部错误: {ex.Message}");
            }
        }
    }
}