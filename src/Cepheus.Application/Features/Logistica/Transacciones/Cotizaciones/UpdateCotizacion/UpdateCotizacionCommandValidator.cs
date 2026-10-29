// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/UpdateCotizacion/UpdateCotizacionCommandValidator.cs
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.UpdateCotizacion
{
    public class UpdateCotizacionCommandValidator : AbstractValidator<UpdateCotizacionCommand>
    {
        public UpdateCotizacionCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty();
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.FechaLimite).GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("La fecha límite no puede ser anterior a la fecha del sistema.");
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para concurrencia.");
        }
    }
}