using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicacionesByArticulo
{
    public class GetArticuloUbicacionesByArticuloQueryHandler
        : IRequestHandler<GetArticuloUbicacionesByArticuloQuery, List<ArticuloUbicacionesResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloUbicacionesByArticuloQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ArticuloUbicacionesResponse>> Handle(
            GetArticuloUbicacionesByArticuloQuery request,
            CancellationToken cancellationToken)
        {
            var artClean = request.ArticuloCode.Trim();

            return await _uow.Logistica.Maestros.ArticuloPlantaUbicaciones.Query()
                .AsNoTracking()
                .Where(x => x.ArticuloCode == artClean)
                .Select(x => new ArticuloUbicacionesResponse
                {
                    Code = x.ArticuloUbicacion.Code,
                    Name = x.ArticuloUbicacion.Name,
                    PlantaCode = x.ArticuloUbicacion.PlantaCode,
                    Zone = x.ArticuloUbicacion.Zone,
                    Rack = x.ArticuloUbicacion.Rack,
                    Shelf = x.ArticuloUbicacion.Shelf,
                    Locker = x.ArticuloUbicacion.Locker,
                    IsActive = x.ArticuloUbicacion.IsActive,
                    CreatedAt = x.ArticuloUbicacion.CreatedAt,
                    CreatedBy = x.ArticuloUbicacion.CreatedBy,
                    UpdatedAt = x.ArticuloUbicacion.UpdatedAt,
                    UpdatedBy = x.ArticuloUbicacion.UpdatedBy
                })
                .ToListAsync(cancellationToken);
        }
    }
}