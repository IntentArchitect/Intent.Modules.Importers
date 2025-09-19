using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class TechnologyDocument : ITechnologyDocument
    {
        public string UniversityEmail { get; set; } = default!;
        public bool StudentPortalAccess { get; set; }
        public bool LibraryAccess { get; set; }
        public List<ITServiceDocument> ITServices { get; set; } = default!;
        IReadOnlyList<IITServiceDocument> ITechnologyDocument.ITServices => ITServices;

        public Technology ToEntity(Technology? entity = default)
        {
            entity ??= new Technology();

            entity.UniversityEmail = UniversityEmail ?? throw new Exception($"{nameof(entity.UniversityEmail)} is null");
            entity.StudentPortalAccess = StudentPortalAccess;
            entity.LibraryAccess = LibraryAccess;
            entity.ITServices = ITServices.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public TechnologyDocument PopulateFromEntity(Technology entity)
        {
            UniversityEmail = entity.UniversityEmail;
            StudentPortalAccess = entity.StudentPortalAccess;
            LibraryAccess = entity.LibraryAccess;
            ITServices = entity.ITServices.Select(x => ITServiceDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static TechnologyDocument? FromEntity(Technology? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TechnologyDocument().PopulateFromEntity(entity);
        }
    }
}