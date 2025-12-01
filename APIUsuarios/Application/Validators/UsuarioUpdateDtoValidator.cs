using APIUsuarios.Application.DTOs;
using FluentValidation;
using System.Text.RegularExpressions;

namespace APIUsuarios.Application.Validators;

public partial class UsuarioUpdateDtoValidator : AbstractValidator<UsuarioUpdateDto>
{
    public UsuarioUpdateDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .Length(3, 100).WithMessage("O nome deve ter entre 3 e 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail deve ter um formato válido.");

        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .Must(BeAtLeast18YearsOld).WithMessage("O usuário deve ter pelo menos 18 anos.");

        RuleFor(x => x.Telefone)
            .Matches(TelefoneRegex())
            .When(x => !string.IsNullOrWhiteSpace(x.Telefone))
            .WithMessage("O telefone deve estar no formato (XX) XXXXX-XXXX.");

        RuleFor(x => x.Ativo)
            .NotNull().WithMessage("O campo Ativo é obrigatório.");
    }

    private static bool BeAtLeast18YearsOld(DateTime dataNascimento)
    {
        var today = DateTime.Today;
        var age = today.Year - dataNascimento.Year;
        
        if (dataNascimento.Date > today.AddYears(-age))
            age--;

        return age >= 18;
    }

    [GeneratedRegex(@"^(\+55\s?)?(\(?\d{2}\)?\s?)?9?\d{4}-?\s?\d{4}$")]
    private static partial Regex TelefoneRegex();
}
