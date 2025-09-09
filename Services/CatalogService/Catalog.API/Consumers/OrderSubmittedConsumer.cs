using Catalog.API.Events;   // 🔹 Aquí usas el contrato
using MassTransit;

namespace Catalog.API.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmitted>
{
    public async Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        var message = context.Message;

        // Aquí implementas la lógica al recibir un evento
        Console.WriteLine($"Pedido recibido en Catalog: {message.OrderId} con producto {message.ProductId}");

        // Ejemplo: podrías actualizar stock, etc.
        await Task.CompletedTask;
    }
}
