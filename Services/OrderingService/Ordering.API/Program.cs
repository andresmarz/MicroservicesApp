using Ordering.API.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.DTOs;
using Ordering.Application.DTOs.External;
using Ordering.Application.Interfaces;
using Ordering.Application.Services;
using Ordering.Application.Services.Orchestration;
using Ordering.Domain.Entities;
using Ordering.Domain.Interfaces;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.HttpClients;
using Ordering.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1) Leer config de Rabbit desde variables/env/appsettings
var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? builder.Configuration["RabbitMq__Host"] ?? "rabbitmq";
var rabbitUser = builder.Configuration["RabbitMq:Username"] ?? builder.Configuration["RabbitMq__Username"] ?? "guest";
var rabbitPass = builder.Configuration["RabbitMq:Password"] ?? builder.Configuration["RabbitMq__Password"] ?? "guest";

// 2) Registrar MassTransit + consumidor
builder.Services.AddMassTransit(x =>
{
    
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


app.MapPost("/api/orders/create-from-catalog/{productId:guid}", async (
    Guid productId,
    int quantity,                                  // viene por query ?quantity=2
    OrderingDbContext db,
    ICatalogServiceHttpClient catalogClient,
    MassTransit.IPublishEndpoint publisher
) =>
{
    // 1) Consultar el producto en Catalog
    var product = await catalogClient.GetProductByIdAsync(productId);
    if (product is null)
        return Results.NotFound("Producto no encontrado en Catalog");

    // 2) Crear la entidad Order (tu modelo actual)
    var order = new Order
    {
        Id = Guid.NewGuid(),
        CustomerName = "Anon",                     // pon aquí si luego quieres leerlo del body
        Product = product.Name,                    // tu entidad guarda el nombre, no el Id
        Quantity = quantity,
        TotalPrice = product.Price * quantity,
        OrderDate = DateTime.UtcNow
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    // 3) Publicar evento asíncrono (aquí sí usamos el Id y Price del producto)
    await publisher.Publish(new OrderSubmitted(
        order.Id,
        product.Id,                                // <- Id del Catalog
        order.Quantity,
        product.Price
    ));

    // 4) Devolver respuesta (puedes usar tu OrderDto si quieres)
    var response = new
    {
        order.Id,
        order.CustomerName,
        Product = order.Product,
        order.Quantity,
        order.TotalPrice,
        order.OrderDate
    };

    return Results.Created($"/api/orders/{order.Id}", response);
});



app.Run();
