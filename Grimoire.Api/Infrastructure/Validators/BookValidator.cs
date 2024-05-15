using FluentValidation;
using Grimoire.Api.Models;

namespace Grimoire.Api.Infrastructure.Validators;

public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(book => book.Isbn)
            .NotEmpty()
            .Matches(@"^\d{13}$")
            .WithMessage("ISBN must be 13 digits long.");

        RuleFor(book => book.Title)
            .NotEmpty();
    }
}
