using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class ActivityDocument : IActivityDocument
    {
        public List<SportDocument> Sports { get; set; } = default!;
        IReadOnlyList<ISportDocument> IActivityDocument.Sports => Sports;
        public List<OrganizationDocument> Organizations { get; set; } = default!;
        IReadOnlyList<IOrganizationDocument> IActivityDocument.Organizations => Organizations;
        public List<HonorDocument> Honors { get; set; } = default!;
        IReadOnlyList<IHonorDocument> IActivityDocument.Honors => Honors;

        public Activity ToEntity(Activity? entity = default)
        {
            entity ??= new Activity();
            entity.Sports = Sports.Select(x => x.ToEntity()).ToList();
            entity.Organizations = Organizations.Select(x => x.ToEntity()).ToList();
            entity.Honors = Honors.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public ActivityDocument PopulateFromEntity(Activity entity)
        {
            Sports = entity.Sports.Select(x => SportDocument.FromEntity(x)!).ToList();
            Organizations = entity.Organizations.Select(x => OrganizationDocument.FromEntity(x)!).ToList();
            Honors = entity.Honors.Select(x => HonorDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static ActivityDocument? FromEntity(Activity? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ActivityDocument().PopulateFromEntity(entity);
        }
    }
}