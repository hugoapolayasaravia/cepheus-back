using System.Collections.Generic;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.DeleteArticuloPlantaUbicaciones
{
    public class DeleteArticuloPlantaUbicacionesCommandHandler : IRequestHandler<DeleteArticuloPlantaUbicacionesCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteArticuloPlantaUbicacionesCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteArticuloPlantaUbicacionesCommand request, CancellationToken cancellationToken)
        {
            var artClean = request.ArticuloCode.Trim();
            var plantaClean = request.PlantaCode.Trim();

            var relacion = await _uow.Logistica.Maestros.ArticuloPlantaUbicaciones.Query()
                .FirstOrDefaultAsync(x => x.ArticuloCode == artClean && x.PlantaCode == plantaClean, cancellationToken);

            if (relacion is null)
            {
                throw new KeyNotFoundException(
                    $"No existe asignación de ubicación para el artículo '{request.ArticuloCode}' en la planta '{request.PlantaCode}'.");
            }

            _uow.Logistica.Maestros.ArticuloPlantaUbicaciones.Remove(relacion);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}