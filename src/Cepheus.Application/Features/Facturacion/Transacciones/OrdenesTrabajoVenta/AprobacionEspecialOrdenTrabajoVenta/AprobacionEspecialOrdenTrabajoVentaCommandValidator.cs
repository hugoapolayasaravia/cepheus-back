using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AprobacionEspecialOrdenTrabajoVenta;

public sealed class AprobacionEspecialOrdenTrabajoVentaCommandValidator : AbstractValidator<AprobacionEspecialOrdenTrabajoVentaCommand>
{
    public AprobacionEspecialOrdenTrabajoVentaCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull();
        RuleFor(x => x.Data.Observations).NotEmpty().MaximumLength(500).When(x => x.Data is not null)
            .WithMessage("El sustento de la aprobación especial es obligatorio.");
    }
}
