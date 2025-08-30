using Microsoft.EntityFrameworkCore;
using Ordering.Application.Interfaces;
using Ordering.Application.Services;
using Ordering.Application.Services.Orchestration;
using Ordering.Domain.Interfaces;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.HttpClients;
using Ordering.Infrastructure.Repositories;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// 1) Leer config de Rabbit desde variables/env/appsettings
var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? builder.Configuration["RabbitMq__Host"] ?? "rabbitmq";
var rabbitUser = builder.Configuration["RabbitMq:Username"] ?? builder.Configuration["RabbitMq__Username"] ?? "guest";
var rabbitPass = builder.Configuration["RabbitMq:Password"] ?? builder.Configuration["RabbitMq__Password"] ?? "guest";


// Add services to the container.
// Configure EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<OrderingDbContext>(options =>
    options.UseSqlServer(connectionString));

// Dependency Injection
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderOrchestrationService, OrderOrchestrationService>();


//Registering an HttpClient
builder.Services.AddHttpClient<ICatalogServiceHttpClient, CatalogServiceHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CatalogService:BaseUrl"]!);
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply automated migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseAuthorization();

app.MapControllers();

app.Run();
