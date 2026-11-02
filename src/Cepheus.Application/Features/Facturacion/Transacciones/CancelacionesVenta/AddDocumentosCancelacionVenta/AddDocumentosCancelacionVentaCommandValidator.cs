using Cepheus.Application.Features.Facturacion.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.AddDocumentosCancelacionVenta
{
    public class AddDocumentosCancelacionVentaCommandValidator : AbstractValidator<AddDocumentosCancelacionVentaCommand>
    {
        public AddDocumentosCancelacionVentaCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(10);
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio.");
            RuleFor(x => x.Documents).NotEmpty().WithMessage("Indique al menos un documento.");
            RuleFor(x => x.Documents).Must(d => d.Count <= 500)
                .WithMessage("No se pueden agregar más de 500 documentos a la vez.");

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
