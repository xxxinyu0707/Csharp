using Microsoft.EntityFrameworkCore;
using OrderManagementLogic; // �����߼��������ռ�

var builder = WebApplication.CreateBuilder(args);

// 1. ��ȡ���ݿ������ַ���
var connectionString = builder.Configuration.GetConnectionString("OrderDbConnection");

// 2. ע�� OrderContext (���ݿ�������)
builder.Services.AddDbContext<OrderContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
    );

// 3. ע�� OrderService (ҵ���߼�����)

builder.Services.AddScoped<OrderService>();

// 4. ��ӿ��������� (���ʹ�ÿ�����ģʽ)
builder.Services.AddControllers()
    //  ���� JSON ���л�ѡ�����ѭ������ (��� Order �� OrderDetails �������õ�������)
    .AddJsonOptions(options =>
    {

    });


// 5. ��� Swagger/OpenAPI ���� (�����Ҫ)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ���� HTTP ����ܵ�
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // ���� Swagger �м��
    app.UseSwaggerUI(); // ���� Swagger UI (���� /swagger �鿴 API �ĵ�)
}

app.UseHttpsRedirection(); // ǿ�� HTTPS

app.UseAuthorization(); // ������Ȩ (�����Ҫ)

app.MapControllers(); // ������ӳ�䵽������ Action

app.Run(); // ����Ӧ��