using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Interfaces;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderOrchestrationController : ControllerBase
    {
        private readonly IOrderOrchestrationService _orchestrationService;

        public OrderOrchestrationController(IOrderOrchestrationService orchestrationService)
        {
            _orchestrationService = orchestrationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFromCatalog(Guid productId, int quantity)
        {
            await _orchestrationService.CreateOrderFromCatalogAsync(productId, quantity);
            return Ok();
        }
    }
}
