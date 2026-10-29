using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.CreateArticuloPlantaUbicaciones
{
    public class CreateArticuloPlantaUbicacionesCommandValidator : AbstractValidator<CreateArticuloPlantaUbicacionesCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateArticuloPlantaUbicacionesCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.ArticuloCode)
                .NotEmpty().WithMessage("El código de artículo es obligatorio.")
                .MaximumLength(7).WithMessage("El código de artículo no puede exceder 7 caracteres.")
                .MustAsync(ArticuloExisteAsync).WithMessage("El artículo especificado no existe.");

            RuleFor(x => x.PlantaCode)
                .NotEmpty().WithMessage("El código de planta es obligatorio.")
                .MaximumLength(2).WithMessage("El código de planta no puede exceder 2 caracteres.")
                .MustAsync(PlantaExisteAsync).WithMessage("La planta especificada no existe.");

            RuleFor(x => x.ArticuloUbicacionCode)
                .NotEmpty().WithMessage("El código de ubicación física es obligatorio.")
                .Length(4).WithMessage("El código de ubicación debe tener 4 caracteres.")
                .MustAsync(UbicacionExisteAsync).WithMessage("La ubicación física especificada no existe.");

            RuleFor(x => x)
                .MustAsync(RelacionUnicaAsync).WithMessage("El artículo ya tiene una ubicación asignada en esta planta.");
        }

        private async Task<bool> ArticuloExisteAsync(string articuloCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(articuloCode)) return false;
            var codeClean = articuloCode.Trim();

            return await _uow.Logistica.Maestros.Articulos.Query()
                .AnyAsync(x => x.Code == codeClean, cancellationToken);
        }

        private async Task<bool> PlantaExisteAsync(string plantaCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(plantaCode)) return false;
            var codeClean = plantaCode.Trim();

            return await _uow.Comunes.Plantas.Query()
                .AnyAsync(x => x.Code == codeClean, cancellationToken);
        }

        private async Task<bool> UbicacionExisteAsync(string ubicacionCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ubicacionCode)) return false;
            var codeClean = ubicacionCode.Trim();

            return await _uow.Logistica.Maestros.ArticuloUbicaciones.Query()
                .AnyAsync(x => x.Code == codeClean, cancellationToken);
        }

        private async Task<bool> RelacionUnicaAsync(CreateArticuloPlantaUbicacionesCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.ArticuloCode) || string.IsNullOrWhiteSpace(command.PlantaCode)) return true;

            var artClean = command.ArticuloCode.Trim();
            var plantaClean = command.PlantaCode.Trim();

            return !await _uow.Logistica.Maestros.ArticuloPlantaUbicaciones.Query()
                .AnyAsync(x => x.ArticuloCode == artClean && x.PlantaCode == plantaClean, cancellationToken);
        }
    }
}