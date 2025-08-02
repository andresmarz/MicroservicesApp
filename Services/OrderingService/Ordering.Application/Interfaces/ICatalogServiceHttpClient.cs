using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ordering.Application.DTOs.External;

namespace Ordering.Application.Interfaces
{
    public interface ICatalogServiceHttpClient
    {
        Task<ProductResponseDto?> GetProductByIdAsync(Guid productId);
    }
}
