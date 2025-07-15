using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TestDataGenerator.Application.Orders
{
    public class OrderCreateDto
    {
        public OrderCreateDto()
        {
        }

        public int RefNo { get; set; }
        public DateTime CreatedDate { get; set; }

        public static OrderCreateDto Create(int refNo, DateTime createdDate)
        {
            return new OrderCreateDto
            {
                RefNo = refNo,
                CreatedDate = createdDate
            };
        }
    }
}