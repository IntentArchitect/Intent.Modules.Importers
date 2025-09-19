using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class EnrollmentPrerequisiteDocument : IEnrollmentPrerequisiteDocument
    {
        public bool AllPrerequisitesMet { get; set; }
        public List<WaiverDocument> Waivers { get; set; } = default!;
        IReadOnlyList<IWaiverDocument> IEnrollmentPrerequisiteDocument.Waivers => Waivers;
        public List<PrerequisiteCheckDocument> PrerequisiteChecks { get; set; } = default!;
        IReadOnlyList<IPrerequisiteCheckDocument> IEnrollmentPrerequisiteDocument.PrerequisiteChecks => PrerequisiteChecks;

        public EnrollmentPrerequisite ToEntity(EnrollmentPrerequisite? entity = default)
        {
            entity ??= new EnrollmentPrerequisite();

            entity.AllPrerequisitesMet = AllPrerequisitesMet;
            entity.Waivers = Waivers.Select(x => x.ToEntity()).ToList();
            entity.PrerequisiteChecks = PrerequisiteChecks.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public EnrollmentPrerequisiteDocument PopulateFromEntity(EnrollmentPrerequisite entity)
        {
            AllPrerequisitesMet = entity.AllPrerequisitesMet;
            Waivers = entity.Waivers.Select(x => WaiverDocument.FromEntity(x)!).ToList();
            PrerequisiteChecks = entity.PrerequisiteChecks.Select(x => PrerequisiteCheckDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static EnrollmentPrerequisiteDocument? FromEntity(EnrollmentPrerequisite? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EnrollmentPrerequisiteDocument().PopulateFromEntity(entity);
        }
    }
}