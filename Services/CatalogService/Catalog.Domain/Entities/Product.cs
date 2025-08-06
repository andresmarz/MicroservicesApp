
namespace Catalog.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; } // We use Guid for microservices
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
