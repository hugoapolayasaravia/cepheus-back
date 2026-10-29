using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.UpdateTipoCentroFormacion
{
    public class UpdateTipoCentroFormacionCommandValidator : AbstractValidator<UpdateTipoCentroFormacionCommand>
    {
        public UpdateTipoCentroFormacionCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del tipo de centro de formación es obligatorio.")
                .MaximumLength(20).WithMessage("El código no puede exceder los 20 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de centro de formación es obligatorio.")
                .MaximumLength(150).WithMessage("El nombre no puede exceder los 150 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}