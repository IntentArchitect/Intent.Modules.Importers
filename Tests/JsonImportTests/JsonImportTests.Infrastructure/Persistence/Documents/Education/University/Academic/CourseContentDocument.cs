using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class CourseContentDocument : ICourseContentDocument
    {
        public List<TextBookDocument> TextBooks { get; set; } = default!;
        IReadOnlyList<ITextBookDocument> ICourseContentDocument.TextBooks => TextBooks;
        public List<OnlineResourceDocument> OnlineResources { get; set; } = default!;
        IReadOnlyList<IOnlineResourceDocument> ICourseContentDocument.OnlineResources => OnlineResources;
        public List<ModuleDocument> Modules { get; set; } = default!;
        IReadOnlyList<IModuleDocument> ICourseContentDocument.Modules => Modules;

        public CourseContent ToEntity(CourseContent? entity = default)
        {
            entity ??= new CourseContent();
            entity.TextBooks = TextBooks.Select(x => x.ToEntity()).ToList();
            entity.OnlineResources = OnlineResources.Select(x => x.ToEntity()).ToList();
            entity.Modules = Modules.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public CourseContentDocument PopulateFromEntity(CourseContent entity)
        {
            TextBooks = entity.TextBooks.Select(x => TextBookDocument.FromEntity(x)!).ToList();
            OnlineResources = entity.OnlineResources.Select(x => OnlineResourceDocument.FromEntity(x)!).ToList();
            Modules = entity.Modules.Select(x => ModuleDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static CourseContentDocument? FromEntity(CourseContent? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CourseContentDocument().PopulateFromEntity(entity);
        }
    }
}