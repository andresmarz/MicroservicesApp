using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Catalog.Application.DTOs;

namespace Catalog.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task AddAsync(CreateProductDto dto);
        Task UpdateAsync(Guid id, CreateProductDto dto);
        Task DeleteAsync(Guid id);
    }
}

