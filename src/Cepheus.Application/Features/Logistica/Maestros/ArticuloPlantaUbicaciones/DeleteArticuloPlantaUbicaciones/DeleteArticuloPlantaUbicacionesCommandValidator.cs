using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.DeleteArticuloPlantaUbicaciones
{
    public class DeleteArticuloPlantaUbicacionesCommandValidator : AbstractValidator<DeleteArticuloPlantaUbicacionesCommand>
    {
        public DeleteArticuloPlantaUbicacionesCommandValidator()
        {
            RuleFor(x => x.ArticuloCode)
                .NotEmpty().WithMessage("El código de artículo es obligatorio.");

            RuleFor(x => x.PlantaCode)
                .NotEmpty().WithMessage("El código de planta es obligatorio.");
        }
    }
}