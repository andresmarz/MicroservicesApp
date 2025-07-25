using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Catalog.Application.DTOs;
using Catalog.Application.Interfaces;
using Catalog.Domain.Entities;

namespace Catalog.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _repository;

        private ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repository.GetAllAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock
            });
        }
    }
}
