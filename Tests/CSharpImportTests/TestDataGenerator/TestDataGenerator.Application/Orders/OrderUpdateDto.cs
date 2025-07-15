using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TestDataGenerator.Application.Orders
{
    public class OrderUpdateDto
    {
        public OrderUpdateDto()
        {
        }

        public Guid Id { get; set; }
        public int RefNo { get; set; }
        public DateTime CreatedDate { get; set; }

        public static OrderUpdateDto Create(Guid id, int refNo, DateTime createdDate)
        {
            return new OrderUpdateDto
            {
                Id = id,
                RefNo = refNo,
                CreatedDate = createdDate
            };
        }
    }
}