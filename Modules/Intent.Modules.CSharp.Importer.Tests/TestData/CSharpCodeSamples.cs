using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.Modules.CSharp.Importer.Tests.TestData;

/// <summary>
/// IN-MEMORY C# code samples for testing - NO temp files
/// </summary>
internal static class CSharpCodeSamples
{
    public const string SimpleClass = @"
namespace TestApp.Domain
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}";

    public const string TwoClasses = @"
namespace TestApp.Domain
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}";

    public const string ClassWithMethods = @"
namespace TestApp.Services
{
    public class CustomerService
    {
        public Customer GetCustomer(int id) { return null; }
        public void SaveCustomer(Customer customer) { }
        public void DeleteCustomer(int id) { }
    }

    public class Customer { }
}";

    public const string SimpleInterface = @"
namespace TestApp.Contracts
{
    public interface IRepository
    {
        void Save();
        void Delete();
        object Get(int id);
    }
}";

    public const string SimpleEnum = @"
namespace TestApp.Domain
{
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4
    }
}";

    public const string ClassWithInheritance = @"
namespace TestApp.Domain
{
    public class BaseEntity
    {
        public int Id { get; set; }
    }

    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}";

    public const string ClassImplementingInterface = @"
namespace TestApp.Domain
{
    public interface IEntity
    {
        int Id { get; }
    }

    public class Customer : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}";

    public const string Record = @"
namespace TestApp.Domain
{
    public record PersonRecord(string FirstName, string LastName)
    {
        public string Email { get; set; }
    }
}";

    public const string ClassWithConstructors = @"
namespace TestApp.Domain
{
    public class Order
    {
        public Order() { }
        public Order(int orderId, decimal amount) 
        {
            OrderId = orderId;
            Amount = amount;
        }

        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}";

    public const string MixedTypes = @"
namespace TestApp.Domain
{
    public enum Status
    {
        Active,
        Inactive
    }

    public interface IEntity
    {
        int Id { get; }
    }

    public class Customer : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
    }
}";
}
