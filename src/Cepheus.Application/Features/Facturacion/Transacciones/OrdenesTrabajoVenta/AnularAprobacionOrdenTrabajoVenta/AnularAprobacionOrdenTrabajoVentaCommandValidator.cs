using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AnularAprobacionOrdenTrabajoVenta;

public sealed class AnularAprobacionOrdenTrabajoVentaCommandValidator : AbstractValidator<AnularAprobacionOrdenTrabajoVentaCommand>
{
    public AnularAprobacionOrdenTrabajoVentaCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data is not null, () =>
        {
            RuleFor(x => x.Data.Reason).NotEmpty().MinimumLength(2).MaximumLength(500)
                .WithMessage("El motivo de la anulación de la aprobación es obligatorio (mínimo 2 caracteres).");
            RuleFor(x => x.Data.Observations).MaximumLength(500);
        });
    }
}
