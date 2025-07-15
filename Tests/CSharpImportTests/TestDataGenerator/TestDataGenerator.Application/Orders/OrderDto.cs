using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Application.Common.Mappings;
using TestDataGenerator.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TestDataGenerator.Application.Orders
{
    public class OrderDto : IMapFrom<Order>
    {
        public OrderDto()
        {
        }

        public int RefNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid Id { get; set; }

        public static OrderDto Create(int refNo, DateTime createdDate, Guid id)
        {
            return new OrderDto
            {
                RefNo = refNo,
                CreatedDate = createdDate,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Order, OrderDto>();
        }
    }
}