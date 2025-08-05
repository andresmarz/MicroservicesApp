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
                ProductId = productId,
                ProductName = product.Name,
                Quantity = quantity,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.UtcNow
            };

            await _orderRepository.AddAsync(order);
        }
    }
}
