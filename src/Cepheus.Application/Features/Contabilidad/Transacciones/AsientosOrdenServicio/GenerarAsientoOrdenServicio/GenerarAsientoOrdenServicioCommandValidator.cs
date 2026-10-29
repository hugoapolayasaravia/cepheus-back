using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosOrdenServicio.GenerarAsientoOrdenServicio
{
    public class GenerarAsientoOrdenServicioCommandValidator : AbstractValidator<GenerarAsientoOrdenServicioCommand>
    {
        public GenerarAsientoOrdenServicioCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.OrdenServicioCode).NotEmpty().MaximumLength(10);
        }
    }
}
