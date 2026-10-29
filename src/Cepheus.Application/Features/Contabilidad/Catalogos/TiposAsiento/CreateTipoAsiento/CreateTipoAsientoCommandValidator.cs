using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAsiento.CreateTipoAsiento
{
    public class CreateTipoAsientoCommandValidator : AbstractValidator<CreateTipoAsientoCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoAsientoCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de asiento con esa descripción.");

            RuleFor(x => x.ExchangeRateType)
                .IsInEnum().WithMessage("El tipo de cambio no es válido.");

            RuleFor(x => x.Valuation)
                .IsInEnum().WithMessage("La valorización no es válida.");

        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Contabilidad.Catalogos.TiposAsiento.Query()
                .AnyAsync(s => s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
