using FluentValidation;

namespace DevKit.Example.Application;

public sealed class ExampleQueryValidator : AbstractValidator<ExampleQuery>
{
    public ExampleQueryValidator()
    {
        RuleFor(x => x.TodoId)
            .NotEmpty()
            .MinimumLength(2);
    }
}
