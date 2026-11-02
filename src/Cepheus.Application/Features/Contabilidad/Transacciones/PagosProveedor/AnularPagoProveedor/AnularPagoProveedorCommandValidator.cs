using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.AnularPagoProveedor
{
    public class AnularPagoProveedorCommandValidator : AbstractValidator<AnularPagoProveedorCommand>
    {
        public AnularPagoProveedorCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código de operación es obligatorio.");

            RuleFor(x => x)
                .Must(x => x.AnularOperacion || x.AnularAsiento || x.LiberarCheque)
                .WithMessage("Indique qué desea anular: la operación, el asiento o liberar el cheque.");
        }
    }
}
