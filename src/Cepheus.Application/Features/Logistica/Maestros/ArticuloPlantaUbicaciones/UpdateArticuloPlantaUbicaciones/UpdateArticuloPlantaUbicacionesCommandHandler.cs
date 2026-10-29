using System.Collections.Generic;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.UpdateArticuloPlantaUbicaciones
{
    public class UpdateArticuloPlantaUbicacionesCommandHandler
        : IRequestHandler<UpdateArticuloPlantaUbicacionesCommand, ArticuloPlantaUbicacionesResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateArticuloPlantaUbicacionesCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloPlantaUbicacionesResponse> Handle(
            UpdateArticuloPlantaUbicacionesCommand request,
            CancellationToken cancellationToken)
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

            relacion.ArticuloUbicacionCode = request.ArticuloUbicacionCode.Trim();

            await _uow.SaveChangesAsync(cancellationToken);

            return await _uow.Logistica.Maestros.ArticuloPlantaUbicaciones.Query()
                .AsNoTracking()
                .Where(x => x.ArticuloCode == artClean && x.PlantaCode == plantaClean)
                .Select(x => new ArticuloPlantaUbicacionesResponse
                {
                    ArticuloCode = x.ArticuloCode,
                    ArticuloName = x.Articulo.Name,
                    PlantaCode = x.PlantaCode,
                    PlantaName = x.Planta.Name,
                    ArticuloUbicacionCode = x.ArticuloUbicacionCode,
                    ArticuloUbicacionName = x.ArticuloUbicacion.Name,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy
                })
                .FirstAsync(cancellationToken);
        }
    }
}