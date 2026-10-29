using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.CreateTipoBien
{
    public class CreateTipoBienCommandValidator : AbstractValidator<CreateTipoBienCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoBienCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("La descripción es obligatorio.")
                .MaximumLength(100).WithMessage("La descripción no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un Tipo de bien con ese nombre.");

            RuleFor(x => x.DetractionRate)
                .InclusiveBetween(0m, 100m).WithMessage("La tasa de detracción debe estar entre 0 y 100.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.TiposBien.Query()
                .AnyAsync(s => s.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
