using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.CreateArticuloPlantaUbicaciones
{
    public class CreateArticuloPlantaUbicacionesCommandHandler
        : IRequestHandler<CreateArticuloPlantaUbicacionesCommand, ArticuloPlantaUbicacionesResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateArticuloPlantaUbicacionesCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloPlantaUbicacionesResponse> Handle(
            CreateArticuloPlantaUbicacionesCommand request,
            CancellationToken cancellationToken)
        {
            var artClean = request.ArticuloCode.Trim();
            var plantaClean = request.PlantaCode.Trim();
            var ubicacionClean = request.ArticuloUbicacionCode.Trim();

            var entidad = new ArticuloPlantaUbicacion
            {
                ArticuloCode = artClean,
                PlantaCode = plantaClean,
                ArticuloUbicacionCode = ubicacionClean
            };

            await _uow.Logistica.Maestros.ArticuloPlantaUbicaciones.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            var result = await _uow.Logistica.Maestros.ArticuloPlantaUbicaciones.Query()
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

            return result;
        }
    }
}