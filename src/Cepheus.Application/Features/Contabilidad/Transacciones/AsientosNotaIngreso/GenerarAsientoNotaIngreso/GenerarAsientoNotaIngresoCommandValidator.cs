using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GenerarAsientoNotaIngreso
{
    public class GenerarAsientoNotaIngresoCommandValidator : AbstractValidator<GenerarAsientoNotaIngresoCommand>
    {
        public GenerarAsientoNotaIngresoCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.NotaIngresoCode).NotEmpty().MaximumLength(10);
        }
    }
}
