using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.GenerarChequeVenta
{
    public class GenerarChequeVentaCommandValidator : AbstractValidator<GenerarChequeVentaCommand>
    {
        public GenerarChequeVentaCommandValidator()
        {
            RuleFor(x => x.ClienteCode).NotEmpty().MaximumLength(5);
            RuleFor(x => x.BancoCode).NotEmpty().MaximumLength(3);
            RuleFor(x => x.Number).NotEmpty().MaximumLength(15);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("La versión de la fila es obligatoria.");

            RuleFor(x => x.OperationNumber)
                .NotEmpty().WithMessage("Debe indicar el número de operación.")
                .MaximumLength(20).WithMessage("El número de operación no puede exceder los 20 caracteres.");

            RuleFor(x => x.OperationDate).NotEmpty().WithMessage("Debe indicar la fecha de la operación.");
        }
    }
}
