using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.SetPagoProveedorDocumentos
{
    public class SetPagoProveedorDocumentosCommandValidator : AbstractValidator<SetPagoProveedorDocumentosCommand>
    {
        public SetPagoProveedorDocumentosCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código de operación es obligatorio.");

            RuleFor(x => x.Documentos).NotNull().WithMessage("La lista de documentos es obligatoria.");

            RuleForEach(x => x.Documentos).ChildRules(d =>
            {
                d.RuleFor(x => x.ComprobantePagoCode).NotEmpty().WithMessage("El tipo de documento es obligatorio.")
                    .MaximumLength(2).WithMessage("El tipo de documento no puede exceder los 2 caracteres.");
                d.RuleFor(x => x.NumeroDocumento).NotEmpty().WithMessage("El número de documento es obligatorio.")
                    .MaximumLength(15).WithMessage("El número de documento no puede exceder los 15 caracteres.");
            });

            RuleFor(x => x.Documentos)
                .Must(l => l == null || l.Select(d => ((d.ComprobantePagoCode ?? string.Empty).Trim().ToUpper(), (d.NumeroDocumento ?? string.Empty).Trim())).Distinct().Count() == l.Count)
                .WithMessage("Hay documentos repetidos en la lista.");
        }
    }
}
