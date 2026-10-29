using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.UpdateTipoAsiento
{
    public class UpdateTipoAsientoCommandValidator : AbstractValidator<UpdateTipoAsientoCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoAsientoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .Length(2).WithMessage("El código debe tener 2 caracteres.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.");

            RuleFor(x => x.ExchangeRateType)
                .IsInEnum().WithMessage("El tipo de cambio no es válido.");

            RuleFor(x => x.Valuation)
                .IsInEnum().WithMessage("La valorización no es válida.");

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
