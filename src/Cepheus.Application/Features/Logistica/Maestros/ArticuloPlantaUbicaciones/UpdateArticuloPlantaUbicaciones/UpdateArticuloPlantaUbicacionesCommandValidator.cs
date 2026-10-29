using System.Collections.Generic;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.UpdateArticuloPlantaUbicaciones
{
    public class UpdateArticuloPlantaUbicacionesCommandValidator : AbstractValidator<UpdateArticuloPlantaUbicacionesCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateArticuloPlantaUbicacionesCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.ArticuloCode)
                .NotEmpty().WithMessage("El código de artículo es obligatorio.");

            RuleFor(x => x.PlantaCode)
                .NotEmpty().WithMessage("El código de planta es obligatorio.");

            RuleFor(x => x.ArticuloUbicacionCode)
                .NotEmpty().WithMessage("El código de ubicación física es obligatorio.")
                .Length(4).WithMessage("El código de ubicación debe tener 4 caracteres.")
                .MustAsync(UbicacionExisteAsync).WithMessage("La ubicación física especificada no existe.");
        }

        private async Task<bool> UbicacionExisteAsync(string ubicacionCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(ubicacionCode)) return false;
            var codeClean = ubicacionCode.Trim();

            return await _uow.Logistica.Maestros.ArticuloUbicaciones.Query()
                .AnyAsync(x => x.Code == codeClean, cancellationToken);
        }
    }
}