using System.Collections.Generic;
using System.Linq;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicacionesByPlanta
{
    public class GetArticuloUbicacionesByPlantaQueryHandler
        : IRequestHandler<GetArticuloUbicacionesByPlantaQuery, List<ArticuloUbicacionesResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloUbicacionesByPlantaQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ArticuloUbicacionesResponse>> Handle(
            GetArticuloUbicacionesByPlantaQuery request,
            CancellationToken cancellationToken)
        {
            var plantaCodeClean = request.PlantaCode.Trim();

            return await _uow.Logistica.Maestros.ArticuloUbicaciones.Query()
                .AsNoTracking()
                .Where(x => x.PlantaCode == plantaCodeClean)
                .Select(x => new ArticuloUbicacionesResponse
                {
                    Code = x.Code,
                    Name = x.Name,
                    PlantaCode = x.PlantaCode,
                    Zone = x.Zone,
                    Rack = x.Rack,
                    Shelf = x.Shelf,
                    Locker = x.Locker,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy
                })
                .ToListAsync(cancellationToken);
        }
    }
}