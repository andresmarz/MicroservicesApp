using Ordering.Application.DTOs.External;
using Ordering.Application.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Domain.Interfaces;

namespace Ordering.Application.Services.Orchestration
{
    public class OrderOrchestrationService : IOrderOrchestrationService
    {
        private readonly ICatalogServiceHttpClient _catalogClient;
        private readonly IOrderRepository _orderRepository;

        public OrderOrchestrationService(
            ICatalogServiceHttpClient catalogClient,
            IOrderRepository orderRepository)
        {
            _catalogClient = catalogClient;
            _orderRepository = orderRepository;
        }

        public async Task CreateOrderFromCatalogAsync(Guid productId, int quantity)
        {
            var product = await _catalogClient.GetProductByIdAsync(productId);
            if (product is null)
                throw new Exception("Producto no encontrado en CatalogService.");

            var totalPrice = product.Price * quantity;

            var order = new Order
            {
                Id = Guid.NewGuid(),
                Product = product.Name,         // Usamos 'Product' como nombre
                Quantity = quantity,
                TotalPrice = totalPrice,
                CustomerName = "Cliente Genérico", // O luego puedes hacerlo dinámico
                OrderDate = DateTime.UtcNow     // Ya lo hace por defecto, pero por claridad lo dejamos
            };


            await _orderRepository.AddAsync(order);
        }
    }
}
