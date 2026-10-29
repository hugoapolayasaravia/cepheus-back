using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.CreateSegmentoVentas
{
    public class CreateSegmentoVentasCommandValidator : AbstractValidator<CreateSegmentoVentasCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateSegmentoVentasCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del segmento de ventas es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un segmento de ventas con ese nombre.");
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Facturacion.Catalogos.SegmentosVentas.Query()
                .AnyAsync(t => t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
