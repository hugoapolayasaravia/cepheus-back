using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.AnularOperacionChequeVenta
{
    public class AnularOperacionChequeVentaCommandValidator : AbstractValidator<AnularOperacionChequeVentaCommand>
    {
        public AnularOperacionChequeVentaCommandValidator()
        {
            RuleFor(x => x.ClienteCode).NotEmpty().MaximumLength(5);
            RuleFor(x => x.BancoCode).NotEmpty().MaximumLength(3);
            RuleFor(x => x.Number).NotEmpty().MaximumLength(15);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("La versión de la fila es obligatoria.");
        }
    }
}
