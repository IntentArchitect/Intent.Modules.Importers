using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class AcademicInfoDocument : IAcademicInfoDocument
    {
        public DateTime AdmissionDate { get; set; }
        public string StudentType { get; set; } = default!;
        public string AcademicStatus { get; set; } = default!;
        public string ClassLevel { get; set; } = default!;
        public DateTime ExpectedGraduationDate { get; set; }
        public List<MajorProgramDocument> MajorPrograms { get; set; } = default!;
        IReadOnlyList<IMajorProgramDocument> IAcademicInfoDocument.MajorPrograms => MajorPrograms;
        public GPADocument GPA { get; set; } = default!;
        IGPADocument IAcademicInfoDocument.GPA => GPA;
        public AcademicAdvisorDocument AcademicAdvisor { get; set; } = default!;
        IAcademicAdvisorDocument IAcademicInfoDocument.AcademicAdvisor => AcademicAdvisor;

        public AcademicInfo ToEntity(AcademicInfo? entity = default)
        {
            entity ??= new AcademicInfo();

            entity.AdmissionDate = AdmissionDate;
            entity.StudentType = StudentType ?? throw new Exception($"{nameof(entity.StudentType)} is null");
            entity.AcademicStatus = AcademicStatus ?? throw new Exception($"{nameof(entity.AcademicStatus)} is null");
            entity.ClassLevel = ClassLevel ?? throw new Exception($"{nameof(entity.ClassLevel)} is null");
            entity.ExpectedGraduationDate = ExpectedGraduationDate;
            entity.MajorPrograms = MajorPrograms.Select(x => x.ToEntity()).ToList();
            entity.GPA = GPA.ToEntity() ?? throw new Exception($"{nameof(entity.GPA)} is null");
            entity.AcademicAdvisor = AcademicAdvisor.ToEntity() ?? throw new Exception($"{nameof(entity.AcademicAdvisor)} is null");

            return entity;
        }

        public AcademicInfoDocument PopulateFromEntity(AcademicInfo entity)
        {
            AdmissionDate = entity.AdmissionDate;
            StudentType = entity.StudentType;
            AcademicStatus = entity.AcademicStatus;
            ClassLevel = entity.ClassLevel;
            ExpectedGraduationDate = entity.ExpectedGraduationDate;
            MajorPrograms = entity.MajorPrograms.Select(x => MajorProgramDocument.FromEntity(x)!).ToList();
            GPA = GPADocument.FromEntity(entity.GPA)!;
            AcademicAdvisor = AcademicAdvisorDocument.FromEntity(entity.AcademicAdvisor)!;

            return this;
        }

        public static AcademicInfoDocument? FromEntity(AcademicInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AcademicInfoDocument().PopulateFromEntity(entity);
        }
    }
}