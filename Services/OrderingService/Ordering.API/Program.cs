using Microsoft.EntityFrameworkCore;
using Ordering.Application.Interfaces;
using Ordering.Application.Services;
using Ordering.Application.Services.Orchestration;
using Ordering.Domain.Interfaces;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.HttpClients;
using Ordering.Infrastructure.Repositories;
using MassTransit;
using EventBus.Contracts;
using Catalog.API.Consumers;   // ?? Para que reconozca OrderSubmittedConsumer


var builder = WebApplication.CreateBuilder(args);

// 1) Leer config de Rabbit desde variables/env/appsettings
var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? builder.Configuration["RabbitMq__Host"] ?? "rabbitmq";
var rabbitUser = builder.Configuration["RabbitMq:Username"] ?? builder.Configuration["RabbitMq__Username"] ?? "guest";
var rabbitPass = builder.Configuration["RabbitMq:Password"] ?? builder.Configuration["RabbitMq__Password"] ?? "guest";

// 2) Registrar MassTransit + consumidor
builder.Services.AddMassTransit(x =>
{
    // 3) Registramos el consumer
    x.AddConsumer<OrderSubmittedConsumer>();

    // 4) Configuración del transporte
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        // 5) Crea/Configura la cola para OrderSubmittedConsumer automáticamente
        cfg.ConfigureEndpoints(context);
    });
});

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



app.MapPost("/api/orders", async (
    /* tus dependencias actuales, por ejemplo: */
    OrderingDbContext db,
    MassTransit.IPublishEndpoint publisher, // 6) MassTransit publisher para publicar eventos
    OrderDto req // tu DTO
) =>
{
    // 7) lógica actual de crear order (validar, consultar producto, etc.)
    var order = new Order
    {
        Id = Guid.NewGuid(),
        ProductId = req.ProductId,
        Quantity = req.Quantity,
        UnitPrice = req.UnitPrice, // o el precio obtenido del Catalog
        TotalPrice = req.UnitPrice * req.Quantity,
        CreatedAt = DateTime.UtcNow
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    // 8) Publicar el evento asíncrono
    await publisher.Publish(new OrderSubmitted(order.Id, order.ProductId, order.Quantity, order.UnitPrice));

    return Results.Created($"/api/orders/{order.Id}", new { order.Id });
});


app.Run();
