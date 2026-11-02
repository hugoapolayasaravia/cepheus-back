using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.UpdateNoAfectoRetencion
{
    public class UpdateNoAfectoRetencionCommandValidator : AbstractValidator<UpdateNoAfectoRetencionCommand>
    {
        public UpdateNoAfectoRetencionCommandValidator()
        {
            RuleFor(x => x.Ruc)
                .NotEmpty().WithMessage("El RUC es obligatorio.")
                .Length(11).WithMessage("El RUC debe tener 11 caracteres.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("El tipo es obligatorio.")
                .Length(1).WithMessage("El tipo debe tener 1 carácter.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(70).WithMessage("El nombre no puede exceder los 70 caracteres.");

            RuleFor(x => x.Resolution)
                .NotEmpty().WithMessage("La resolución es obligatoria.")
                .MaximumLength(50).WithMessage("La resolución no puede exceder los 50 caracteres.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
