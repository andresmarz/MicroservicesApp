namespace Ordering.Application.DTOs
{
    public class CreateOrderDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Quantity { get; set; }      
    }
}
