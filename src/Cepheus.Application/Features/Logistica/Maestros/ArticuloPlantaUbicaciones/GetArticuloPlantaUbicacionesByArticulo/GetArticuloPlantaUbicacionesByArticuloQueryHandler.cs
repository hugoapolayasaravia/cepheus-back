using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.GetArticuloPlantaUbicacionesByArticulo
{
    public class GetArticuloPlantaUbicacionesByArticuloQueryHandler
        : IRequestHandler<GetArticuloPlantaUbicacionesByArticuloQuery, List<ArticuloPlantaUbicacionesResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloPlantaUbicacionesByArticuloQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ArticuloPlantaUbicacionesResponse>> Handle(
            GetArticuloPlantaUbicacionesByArticuloQuery request,
            CancellationToken cancellationToken)
        {
            var artClean = request.ArticuloCode.Trim();

            return await _uow.Logistica.Maestros.ArticuloPlantaUbicaciones.Query()
                .AsNoTracking()
                .Where(x => x.ArticuloCode == artClean)
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
                .ToListAsync(cancellationToken);
        }
    }
}