using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.CreateCancelacionVenta
{
    public class CreateCancelacionVentaCommandValidator : AbstractValidator<CreateCancelacionVentaCommand>
    {
        public CreateCancelacionVentaCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().Length(2).WithMessage("El código de planta debe tener 2 caracteres.");
            RuleFor(x => x.ClienteCode).NotEmpty().MaximumLength(5).WithMessage("El cliente es obligatorio.");
            RuleFor(x => x.CobradorCode).NotEmpty().MaximumLength(4).WithMessage("El cobrador es obligatorio.");
            RuleFor(x => x.MonedaCode).NotEmpty().Length(3).WithMessage("La moneda es obligatoria (ISO, ej. PEN).");
            RuleFor(x => x.TipoPagoCode).NotEmpty().Length(2).WithMessage("El tipo de pago es obligatorio.");
            RuleFor(x => x.Date).NotEmpty().WithMessage("La fecha es obligatoria.");
            RuleFor(x => x.TotalAmount).GreaterThanOrEqualTo(0).WithMessage("El importe no puede ser negativo.");
            RuleFor(x => x.PaymentNumber).MaximumLength(15);
            RuleFor(x => x.AccountNumber).MaximumLength(20);
            RuleFor(x => x.BancoDocCode).MaximumLength(3);
            RuleFor(x => x.BancoDepCode).MaximumLength(3);
            RuleFor(x => x.OperationNumber).MaximumLength(10);
            RuleFor(x => x.Description).MaximumLength(100);

            RuleFor(x => x.Documents).Must(d => d.Count <= 500)
                .WithMessage("No se pueden cancelar más de 500 documentos a la vez.");

            RuleForEach(x => x.Documents).ChildRules(d =>
            {
                d.RuleFor(i => i.TypeCode)
                    .Must(t => CobranzaTipoDocumento.Todos.Contains(t))
                    .WithMessage("El tipo de documento debe ser 01, 03, 07, 08, 10 u 11.");
                d.RuleFor(i => i.Number).NotEmpty().MaximumLength(15);
                d.RuleFor(i => i.Amount).GreaterThan(0).When(i => i.Amount.HasValue)
                    .WithMessage("El importe debe ser mayor a 0 (las notas de crédito restan solas).");
            });
        }
    }
}
