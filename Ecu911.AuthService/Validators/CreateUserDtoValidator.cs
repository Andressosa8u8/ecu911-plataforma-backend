using Ecu911.AuthService.DTOs;
using FluentValidation;

namespace Ecu911.AuthService.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MinimumLength(4).WithMessage("El nombre de usuario debe tener al menos 4 caracteres.")
            .MaximumLength(50).WithMessage("El nombre de usuario no puede superar los 50 caracteres.");

        RuleFor(x => x.Nombres)
            .NotEmpty().WithMessage("Los nombres son obligatorios.")
            .MaximumLength(100);

        RuleFor(x => x.Apellidos)
            .NotEmpty().WithMessage("Los apellidos son obligatorios.")
            .MaximumLength(100);

        RuleFor(x => x.Cedula)
            .NotEmpty().WithMessage("La cédula es obligatoria.")
            .Matches("^[0-9]{10,13}$").WithMessage("La cédula debe contener entre 10 y 13 dígitos numéricos.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
            .MaximumLength(150);

        RuleFor(x => x.Telefono)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .MaximumLength(30);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches("[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches("[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula.")
            .Matches("[0-9]").WithMessage("La contraseña debe contener al menos un número.");

        RuleFor(x => x.Cargo)
            .NotEmpty().WithMessage("El cargo es obligatorio.")
            .MaximumLength(150);

        RuleFor(x => x.ProvinciaId)
            .GreaterThan(0).WithMessage("La provincia es obligatoria.");

        RuleFor(x => x.CantonId)
            .GreaterThan(0).WithMessage("El cantón es obligatorio.");

        RuleFor(x => x.CentroZonalId)
            .NotEmpty().WithMessage("El centro zonal es obligatorio.");
    }
}
