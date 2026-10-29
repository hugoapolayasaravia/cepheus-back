using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.GetArticuloPlantaUbicacionesByKey
{
    public class GetArticuloPlantaUbicacionesByKeyQueryHandler
        : IRequestHandler<GetArticuloPlantaUbicacionesByKeyQuery, ArticuloPlantaUbicacionesResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloPlantaUbicacionesByKeyQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloPlantaUbicacionesResponse> Handle(
            GetArticuloPlantaUbicacionesByKeyQuery request,
            CancellationToken cancellationToken)
        {
            var artClean = request.ArticuloCode.Trim();
            var plantaClean = request.PlantaCode.Trim();

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
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
            {
                throw new KeyNotFoundException(
                    $"No existe registro para el artículo '{request.ArticuloCode}' en la planta '{request.PlantaCode}'.");
            }

            return result;
        }
    }
}