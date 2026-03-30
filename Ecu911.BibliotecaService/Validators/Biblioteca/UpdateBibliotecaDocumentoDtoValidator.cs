using Ecu911.BibliotecaService.DTOs;
using FluentValidation;

namespace Ecu911.BibliotecaService.Validators;

public class UpdateBibliotecaDocumentoDtoValidator : AbstractValidator<UpdateBibliotecaDocumentoDto>
{
    public UpdateBibliotecaDocumentoDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(200).WithMessage("El título no puede superar los 200 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(1000).WithMessage("La descripción no puede superar los 1000 caracteres.");

        RuleFor(x => x.BibliotecaCategoriaId)
            .NotEmpty().WithMessage("El tipo de documento es obligatorio.");
    }
}