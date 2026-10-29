using FluentValidation;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSangre.UpdateTipoSangre
{
    public class UpdateTipoSangreCommandValidator : AbstractValidator<UpdateTipoSangreCommand>
    {
        public UpdateTipoSangreCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código del tipo de sangre es obligatorio.")
                .MaximumLength(10).WithMessage("El código no puede exceder los 10 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del tipo de sangre es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}