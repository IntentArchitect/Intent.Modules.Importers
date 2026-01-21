namespace Intent.Modules.CSharp.Importer.Tests.TestData;

/// <summary>
/// Predefined test code files for various scenarios
/// </summary>
public static class TestCodeFiles
{
    public static string SimpleClass => @"
namespace TestNamespace
{
    public class SimpleEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
";

    public static string InterfaceAndClass => @"
namespace TestNamespace
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
    }

    public class OrderEntity : IRepository<OrderEntity>
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }

        public Task<OrderEntity> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task<IEnumerable<OrderEntity>> GetAllAsync() => throw new NotImplementedException();
        public Task AddAsync(OrderEntity entity) => throw new NotImplementedException();
    }
}
";

    public static string Enum => @"
namespace TestNamespace
{
    public enum ProductType
    {
        Physical = 0,
        Digital = 1,
        Service = 2
    }
}
";

    public static string RecordType => @"
namespace TestNamespace
{
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
}
";

    public static string PartialClass => @"
namespace TestNamespace
{
    public partial class PartialEntity
    {
        public int Id { get; set; }
    }
}
";

    public static string PartialClassPart2 => @"
namespace TestNamespace
{
    public partial class PartialEntity
    {
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
";

    public static string ValueObject => @"
namespace TestNamespace
{
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
}
";

    public static string GenericClass => @"
namespace TestNamespace
{
    public class GenericEntity<T>
    {
        public T Value { get; set; }
        public List<T> Values { get; set; }
    }
}
";

    public static string StaticClass => @"
namespace TestNamespace
{
    public static class HelpersUtility
    {
        public static string FormatName(string value) => value?.ToUpperInvariant() ?? string.Empty;
        public static int ParseNumber(string value) => int.Parse(value);
    }
}
";

    public static string MultipleClasses => @"
namespace TestNamespace
{
    public class Customer
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class OrderLine
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
";

    public static string InterfaceWithMethods => @"
namespace TestNamespace
{
    public interface IOrderService
    {
        Task<OrderEntity> CreateOrderAsync(OrderEntity order);
        Task<OrderEntity> GetOrderAsync(int orderId);
        Task UpdateOrderAsync(OrderEntity order);
        Task DeleteOrderAsync(int orderId);
    }

    public class OrderEntity
    {
        public int Id { get; set; }
    }
}
";

    public static string NestedClass => @"
namespace TestNamespace
{
    public class NestedClass
    {
        public class InnerEntity
        {
            public int Id { get; set; }
        }

        public class AnotherInnerEntity
        {
            public string Name { get; set; }
        }
    }
}
";

    public static string ClassWithAttributes => @"
namespace TestNamespace
{
    [Serializable]
    [System.ComponentModel.DataAnnotations.Schema.Table(""Orders"")]
    public class Order
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(255)]
        public string CustomerName { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
";

    public static string MultipleEnums => @"
namespace TestNamespace
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        PayPal,
        BankTransfer
    }

    public enum ProductCategory
    {
        Electronics,
        Clothing,
        Books,
        Home,
        Sports
    }
}
";
}
