using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Maestros.Choferes.UpdateChofer
{
    public class UpdateChoferCommandValidator : AbstractValidator<UpdateChoferCommand>
    {
        public UpdateChoferCommandValidator()
        {
            RuleFor(x => x.TransportistaCode)
                .NotEmpty().WithMessage("El transportista es obligatorio.")
                .Length(4).WithMessage("El código de transportista debe tener 4 caracteres.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del chofer es obligatorio.")
                .Length(4).WithMessage("El código del chofer debe tener 4 caracteres.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("El nombre del chofer es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.DriverLicenseNumber)
                .NotEmpty().WithMessage("El número de brevete es obligatorio.")
                .MaximumLength(20).WithMessage("El brevete no puede exceder los 20 caracteres.");

            RuleFor(x => x.Observations).MaximumLength(500).WithMessage("Las observaciones no pueden exceder los 500 caracteres.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
