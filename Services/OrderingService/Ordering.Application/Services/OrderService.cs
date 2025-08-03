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
        

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
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
