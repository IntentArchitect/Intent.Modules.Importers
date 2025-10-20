using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.ECommerce.Orders
{
    public class PaymentDto
    {
        public PaymentDto()
        {
            Method = null!;
            Status = null!;
            TransactionId = null!;
        }

        public string Method { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public DateTime PaidAt { get; set; }

        public static PaymentDto Create(string method, string status, string transactionId, DateTime paidAt)
        {
            return new PaymentDto
            {
                Method = method,
                Status = status,
                TransactionId = transactionId,
                PaidAt = paidAt
            };
        }
    }
}