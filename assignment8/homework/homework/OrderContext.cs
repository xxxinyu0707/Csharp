using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;

namespace OrderManagementLogic
{
    // 数据库上下文类，负责与数据库交互
    public class OrderContext : DbContext
    {
        // 定义数据库中的表
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        // 配置数据库连接
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // 格式: "Server=服务器地址;Port=端口号;Database=数据库名;Uid=用户名;Pwd=密码;"
                string connectionString = "Server=localhost;Port=3306;Database=OrderDbHomework;Uid=root;Pwd=你的MySQL密码;"; // <-- 修改这里!

                // 使用 Pomelo MySQL 提供程序 (需要安装 Pomelo.EntityFrameworkCore.MySql 包)
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }
        }

        // 配置实体模型和关系
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 配置 Order 和 OrderDetails 之间的一对多关系
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)       // 一个订单有多个明细
                .WithOne(d => d.Order)           // 一个明细属于一个订单
                .HasForeignKey(d => d.OrderId)   // 外键是 OrderDetails.OrderId
                .OnDelete(DeleteBehavior.Cascade); // 设置级联删除 (删除订单时自动删除其明细)

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerName);

            base.OnModelCreating(modelBuilder);
        }
    }
}