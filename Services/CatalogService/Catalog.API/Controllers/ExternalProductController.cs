using Catalog.Application.Interfaces;
using Catalog.Application.DTOs.External;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/external-product")]
    public class ExternalProductController : ControllerBase
    {
        private readonly IProductQueryService _productQueryService;

        public ExternalProductController(IProductQueryService productQueryService)
        {
            _productQueryService = productQueryService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProductById(Guid id)
        {
            var product = await _productQueryService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}