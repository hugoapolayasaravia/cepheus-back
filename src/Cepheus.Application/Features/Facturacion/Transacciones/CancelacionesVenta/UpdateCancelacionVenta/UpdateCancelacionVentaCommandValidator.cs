using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.UpdateCancelacionVenta
{
    public class UpdateCancelacionVentaCommandValidator : AbstractValidator<UpdateCancelacionVentaCommand>
    {
        public UpdateCancelacionVentaCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
            RuleFor(x => x.CobradorCode).NotEmpty().MaximumLength(4).WithMessage("El cobrador es obligatorio.");
            RuleFor(x => x.BancoDepCode).MaximumLength(3);
            RuleFor(x => x.AccountNumber).MaximumLength(20);
            RuleFor(x => x.OperationNumber).MaximumLength(10);
            RuleFor(x => x.Description).MaximumLength(100);
            RuleFor(x => x.TotalAmount).GreaterThan(0).When(x => x.TotalAmount.HasValue)
                .WithMessage("El importe debe ser mayor a 0.");
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio.");
        }
    }
}
