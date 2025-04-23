// OrderManagementLogic/OrderContext.cs (修改后)
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.Remoting.Contexts;


namespace OrderManagementLogic
{
    public class OrderContext : DbContext
    {
        // 标准构造函数，用于依赖注入
        public OrderContext(DbContextOptions<OrderContext> options) : base(options) { }

        // 定义数据库中的表
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        // 移除或注释掉这个方法，因为连接字符串将在 Program.cs 中配置
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     if (!optionsBuilder.IsConfigured)
        //     {
        //         // string connectionString = "Server=localhost;Port=3306;Database=OrderDbHomework;Uid=root;Pwd=你的MySQL密码;"; // <-- 不在这里配置
        //         // optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        //     }
        // }

        // OnModelCreating 保持不变
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(d => d.Order)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CustomerName);

            // 考虑将 OrderDetails 的主键配置移到这里可能更集中
            modelBuilder.Entity<OrderDetails>()
                .Property(d => d.OrderDetailId)
                .ValueGeneratedOnAdd(); // 明确指定数据库生成

            // 移除 Order.TotalAmount 的映射，因为它有 [NotMapped]
            modelBuilder.Entity<Order>()
               .Ignore(o => o.TotalAmount);


            base.OnModelCreating(modelBuilder);
        }
    }
}