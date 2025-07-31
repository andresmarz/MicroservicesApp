using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


using Ordering.Application.DTOs;
using Ordering.Application.Interfaces;
using Ordering.Domain.Entities;
using Ordering.Domain.Interfaces;
using Ordering.Application.DTOs.External;

namespace Ordering.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderService(IOrderRepository repository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = await _repository.GetAllAsync();
            return orders.Select(order => new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Product = order.Product,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
            });             
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Product = order.Product,
                Quantity = order.Quantity,
                TotalPrice = order.TotalPrice,
                OrderDate = order.OrderDate,
            };
        }

       
        public async Task AddAsync(CreateOrderDto dto)
        {
            var client = _httpClientFactory.CreateClient("Catalog");

            var response = await client.GetAsync($"product/{dto.Id}");
            if (!response.IsSuccessStatusCode)
                throw new Exception("Producto no encontrado en CatalogService");

            var product = await response.Content.ReadFromJsonAsync<ProductResponseDto>();

            if (product == null || product.Stock < dto.Quantity)
                throw new Exception("Producto no disponible o sin stock suficiente");

            var order = new Order
            {
                CustomerName = dto.CustomerName,
                Product = product.Name,
                Quantity = dto.Quantity,
                TotalPrice = product.Price * dto.Quantity,
                OrderDate = DateTime.UtcNow
            };

            await _repository.AddAsync(order);
                    
        }

        public async Task UpdateAsync(Guid id, OrderDto dto)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) return;

            order.CustomerName = dto.CustomerName;
            order.Product = dto.Product;
            order.Quantity = dto.Quantity;
            order.TotalPrice = dto.TotalPrice;

            await _repository.UpdateAsync(order);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
