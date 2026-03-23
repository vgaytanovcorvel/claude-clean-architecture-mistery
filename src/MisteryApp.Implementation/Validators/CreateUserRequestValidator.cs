using FluentValidation;
using MisteryApp.Abstractions.Models.Requests;

namespace MisteryApp.Implementation.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[a-zA-Z0-9_-]+$")
            .WithMessage("Username can only contain letters, numbers, hyphens, and underscores");

        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
    }
}
