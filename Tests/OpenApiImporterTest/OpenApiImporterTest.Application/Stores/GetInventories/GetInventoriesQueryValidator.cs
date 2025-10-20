using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Stores.GetInventories
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetInventoriesQueryValidator : AbstractValidator<GetInventoriesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetInventoriesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}