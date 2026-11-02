using Cepheus.Application.Features.Facturacion.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.GenerarLetrasVarias
{
    public class GenerarLetrasVariasCommandValidator : AbstractValidator<GenerarLetrasVariasCommand>
    {
        public GenerarLetrasVariasCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().Length(2).WithMessage("El código de planta debe tener 2 caracteres.");
            RuleFor(x => x.ClienteCode).NotEmpty().MaximumLength(5).WithMessage("El cliente es obligatorio.");
            RuleFor(x => x.CobradorCode).NotEmpty().MaximumLength(4).WithMessage("El cobrador es obligatorio.");
            RuleFor(x => x.MonedaCode).NotEmpty().Length(3).WithMessage("La moneda es obligatoria (ISO, ej. PEN).");
            RuleFor(x => x.Date).NotEmpty().WithMessage("La fecha es obligatoria.");
            RuleFor(x => x.FirstPaymentDate).NotEmpty().WithMessage("La fecha de la primera cuota es obligatoria.");

            RuleFor(x => x.InstallmentCount)
                .InclusiveBetween(1, 120).WithMessage("La cantidad de letras debe estar entre 1 y 120.");

            RuleFor(x => x.InstallmentType)
                .Must(t => t is "F" or "I").WithMessage("El tipo de cuota debe ser F (fija) o I (con interés).");

            RuleFor(x => x.InitialAmount).GreaterThanOrEqualTo(0).WithMessage("La cuota inicial no puede ser negativa.");
            RuleFor(x => x.Interest).GreaterThanOrEqualTo(0).WithMessage("El interés no puede ser negativo.");
            RuleFor(x => x.Description).MaximumLength(255);

            RuleFor(x => x.Documents)
                .NotEmpty().WithMessage("Debe seleccionar al menos un documento.")
                .Must(d => d.Count <= 500).WithMessage("No se pueden refinanciar más de 500 documentos a la vez.");

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
