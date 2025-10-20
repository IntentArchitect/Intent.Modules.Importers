using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.GetLogins
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetLoginsQueryValidator : AbstractValidator<GetLoginsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetLoginsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Username)
                .NotNull();

            RuleFor(v => v.Password)
                .NotNull();
        }
    }
}