using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.InactivarOrdenTrabajoVenta;

public sealed class InactivarOrdenTrabajoVentaCommandValidator : AbstractValidator<InactivarOrdenTrabajoVentaCommand>
{
    public InactivarOrdenTrabajoVentaCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data is not null, () =>
        {
            RuleFor(x => x.Data.Reason).NotEmpty().MinimumLength(2).MaximumLength(500)
                .WithMessage("El motivo de la inactivación es obligatorio (mínimo 2 caracteres).");
        });
    }
}
