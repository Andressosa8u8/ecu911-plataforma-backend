using Ecu911.RepositorioService.DTOs;
using FluentValidation;

namespace Ecu911.RepositorioService.Validators;

public class CreateDocumentItemDtoValidator : AbstractValidator<CreateDocumentItemDto>
{
    public CreateDocumentItemDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(200).WithMessage("El título no puede superar los 200 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(1000).WithMessage("La descripción no puede superar los 1000 caracteres.");

        RuleFor(x => x.DocumentTypeId)
            .NotEmpty().WithMessage("El tipo de documento es obligatorio.");
    }
}