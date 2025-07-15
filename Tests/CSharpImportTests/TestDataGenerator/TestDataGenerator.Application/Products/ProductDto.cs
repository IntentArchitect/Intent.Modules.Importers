using System;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Application.Common.Mappings;
using TestDataGenerator.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace TestDataGenerator.Application.Products
{
    public class ProductDto : IMapFrom<Product>
    {
        public ProductDto()
        {
            Name = null!;
        }

        public string Name { get; set; }
        public ServiceProductEnum Type { get; set; }
        public Guid Id { get; set; }

        public static ProductDto Create(string name, ServiceProductEnum type, Guid id)
        {
            return new ProductDto
            {
                Name = name,
                Type = type,
                Id = id
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDto>();
        }
    }
}