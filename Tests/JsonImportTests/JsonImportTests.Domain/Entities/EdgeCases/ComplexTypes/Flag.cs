using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Flag
    {
        public Flag()
        {
            IsVerified = null!;
        }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPublic { get; set; }

        public object IsVerified { get; set; }

        public bool RequiresApproval { get; set; }

        public bool HasNotifications { get; set; }
    }
}