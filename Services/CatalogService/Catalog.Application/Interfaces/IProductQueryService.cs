using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Catalog.Application.DTOs.External;

namespace Catalog.Application.Interfaces
{
    public interface IProductQueryService
    {
        Task<ProductResponseDto?> GetByIdAsync(Guid id);
    }
}

