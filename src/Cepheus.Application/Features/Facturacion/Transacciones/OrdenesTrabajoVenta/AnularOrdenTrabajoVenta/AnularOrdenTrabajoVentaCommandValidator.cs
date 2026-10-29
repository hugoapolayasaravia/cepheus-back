using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AnularOrdenTrabajoVenta;

public sealed class AnularOrdenTrabajoVentaCommandValidator : AbstractValidator<AnularOrdenTrabajoVentaCommand>
{
    public AnularOrdenTrabajoVentaCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data is not null, () =>
        {
            RuleFor(x => x.Data.Reason).NotEmpty().MinimumLength(2).MaximumLength(500)
                .WithMessage("El motivo de la anulación es obligatorio (mínimo 2 caracteres).");
        });
    }
}
