using Intent.Modules.Common;

namespace Intent.Modules.CSharp.Importer.Tests.TestData;

/// <summary>
/// Test code scenarios for C# Importer testing
/// </summary>
public static class CSharpCodeScenarios
{
    public class SimpleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class OrderEntity
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CustomerEntity
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public enum ProductType
    {
        Physical = 0,
        Digital = 1,
        Service = 2
    }

    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
    }

    public interface IOrderService
    {
        Task<OrderEntity> CreateOrderAsync(OrderEntity order);
        Task<OrderEntity> GetOrderAsync(int orderId);
    }

    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> GetEqualityComponents();
    }

    public class Money : ValueObject
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
    }

    public record OrderDto
    {
        public int Id { get; init; }
        public int CustomerId { get; init; }
        public decimal TotalAmount { get; init; }
    }

    public record CreateOrderDto
    {
        public int CustomerId { get; init; }
        public decimal TotalAmount { get; init; }
    }

    public partial class PartialEntity
    {
        public int Id { get; set; }
    }

    public partial class PartialEntity
    {
        public string Name { get; set; }
    }

    public class GenericEntity<T>
    {
        public T Value { get; set; }
    }

    public static class HelpersUtility
    {
        public static string FormatName(string value) => value?.ToUpperInvariant() ?? string.Empty;
    }

    public class NestedClass
    {
        public class InnerEntity
        {
            public int Id { get; set; }
        }
    }
}
