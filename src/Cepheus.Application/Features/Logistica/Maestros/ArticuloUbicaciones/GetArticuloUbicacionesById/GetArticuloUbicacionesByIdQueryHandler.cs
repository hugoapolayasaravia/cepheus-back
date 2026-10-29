using System.Collections.Generic;
using System.Linq;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.GetArticuloUbicacionById
{
    public class GetArticuloUbicacionByIdQueryHandler
        : IRequestHandler<GetArticuloUbicacionByIdQuery, ArticuloUbicacionesResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetArticuloUbicacionByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ArticuloUbicacionesResponse> Handle(
            GetArticuloUbicacionByIdQuery request,
            CancellationToken cancellationToken)
        {
            var codeClean = request.Code.Trim();

            var result = await _uow.Logistica.Maestros.ArticuloUbicaciones.Query()
                .AsNoTracking()
                .Where(x => x.Code == codeClean)
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
                .FirstOrDefaultAsync(cancellationToken);

            if (result is null)
            {
                throw new KeyNotFoundException($"No existe la ubicación con código '{request.Code}'.");
            }

            return result;
        }
    }
}