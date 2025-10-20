using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.ECommerce.Orders
{
    public class ItemDto
    {
        public ItemDto()
        {
            ProductName = null!;
        }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public static ItemDto Create(Guid productId, string productName, decimal quantity, decimal unitPrice, decimal totalPrice)
        {
            return new ItemDto
            {
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity,
                UnitPrice = unitPrice,
                TotalPrice = totalPrice
            };
        }
    }
}