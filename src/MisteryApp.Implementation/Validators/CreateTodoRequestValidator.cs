using FluentValidation;
using MisteryApp.Abstractions.Models;

namespace MisteryApp.Implementation.Validators;

public class CreateTodoRequestValidator : AbstractValidator<CreateTodoRequest>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
