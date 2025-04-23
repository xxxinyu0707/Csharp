using Microsoft.EntityFrameworkCore;
using OrderManagementLogic; // 引入逻辑层命名空间

var builder = WebApplication.CreateBuilder(args);

// 1. 读取数据库连接字符串
var connectionString = builder.Configuration.GetConnectionString("OrderDbConnection");

// 2. 注册 OrderContext (数据库上下文)
builder.Services.AddDbContext<OrderContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    );

// 3. 注册 OrderService (业务逻辑服务)

builder.Services.AddScoped<OrderService>();

// 4. 添加控制器服务 (如果使用控制器模式)
builder.Services.AddControllers()
    //  配置 JSON 序列化选项，处理循环引用 (如果 Order 和 OrderDetails 互相引用导致问题)
    .AddJsonOptions(options =>
    {

    });


// 5. 添加 Swagger/OpenAPI 服务 (如果需要)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // 启用 Swagger 中间件
    app.UseSwaggerUI(); // 启用 Swagger UI (访问 /swagger 查看 API 文档)
}

app.UseHttpsRedirection(); // 强制 HTTPS

app.UseAuthorization(); // 启用授权 (如果需要)

app.MapControllers(); // 将请求映射到控制器 Action

app.Run(); // 运行应用