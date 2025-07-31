using Ordering.Application.DTOs;

namespace Ordering.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task AddAsync(CreateOrderDto dto);
        Task UpdateAsync(Guid id, OrderDto dto);
        Task DeleteAsync(Guid id);
    }
}
