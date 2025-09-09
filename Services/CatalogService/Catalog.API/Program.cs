using Catalog.Application.Interfaces;
using Catalog.Application.Services;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Catalog.API.Events;

using MassTransit;
using Microsoft.EntityFrameworkCore;
using Catalog.Infrastructure.Data;          // tu DbContext
using Catalog.API.Consumers;              // el consumer que crearemos


var builder = WebApplication.CreateBuilder(args);

//RabbitMQ part
// 1) Leer config de Rabbit desde variables/env/appsettings
var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? builder.Configuration["RabbitMq__Host"] ?? "rabbitmq";
var rabbitUser = builder.Configuration["RabbitMq:Username"] ?? builder.Configuration["RabbitMq__Username"] ?? "guest";
var rabbitPass = builder.Configuration["RabbitMq:Password"] ?? builder.Configuration["RabbitMq__Password"] ?? "guest";




// Add services to the container.
// Configure EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(connectionString));


// ---- MassTransit: registramos el consumer y el bus RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // registramos el consumer concreto
    x.AddConsumer<OrderSubmittedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        // Cola/endpoint donde Catalog escuchará OrderSubmitted
        cfg.ReceiveEndpoint("order-submitted-queue", e =>
        {
            e.ConfigureConsumer<OrderSubmittedConsumer>(context);
        });
    });
});



// Dependency Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductQueryService, ProductQueryService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply automated migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
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
