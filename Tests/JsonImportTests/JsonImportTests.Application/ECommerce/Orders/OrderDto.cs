using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.ECommerce.Orders
{
    public class OrderDto
    {
        public OrderDto()
        {
            OrderNumber = null!;
            Status = null!;
            ShippingAddress = null!;
            Items = null!;
            Payment = null!;
        }

        public Guid OrderId { get; set; }
        public string OrderNumber { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; }
        public List<ItemDto> Items { get; set; }
        public PaymentDto Payment { get; set; }

        public static OrderDto Create(
            Guid orderId,
            string orderNumber,
            Guid customerId,
            DateTime orderDate,
            string status,
            decimal totalAmount,
            ShippingAddressDto shippingAddress,
            List<ItemDto> items,
            PaymentDto payment)
        {
            return new OrderDto
            {
                OrderId = orderId,
                OrderNumber = orderNumber,
                CustomerId = customerId,
                OrderDate = orderDate,
                Status = status,
                TotalAmount = totalAmount,
                ShippingAddress = shippingAddress,
                Items = items,
                Payment = payment
            };
        }
    }
}