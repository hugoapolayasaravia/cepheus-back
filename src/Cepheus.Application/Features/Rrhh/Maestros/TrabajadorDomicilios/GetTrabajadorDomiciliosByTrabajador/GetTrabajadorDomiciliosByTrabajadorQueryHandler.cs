using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.GetTrabajadorDomiciliosByTrabajador
{
    public class GetTrabajadorDomiciliosByTrabajadorQueryHandler
        : IRequestHandler<GetTrabajadorDomiciliosByTrabajadorQuery, List<TrabajadorDomicilioResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorDomiciliosByTrabajadorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<TrabajadorDomicilioResponse>> Handle(
            GetTrabajadorDomiciliosByTrabajadorQuery request, CancellationToken cancellationToken)
        {
            var trabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant();

            return await _uow.Rrhh.Maestros.TrabajadorDomicilios.Query()
                .AsNoTracking()
                .Where(d => d.TrabajadorCode == trabajadorCode)
                .OrderByDescending(d => d.IsPrimary)
                .ThenBy(d => d.Id)
                .Select(d => new TrabajadorDomicilioResponse
                {
                    Id = d.Id,
                    TrabajadorCode = d.TrabajadorCode,
                    RoadTypeCode = d.RoadTypeCode,
                    StreetName = d.StreetName,
                    StreetNumber = d.StreetNumber,
                    InteriorNumber = d.InteriorNumber,
                    ZoneTypeCode = d.ZoneTypeCode,
                    ZoneName = d.ZoneName,
                    Reference = d.Reference,
                    UbigeoCode = d.UbigeoCode,
                    IsPrimary = d.IsPrimary,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    CreatedBy = d.CreatedBy,
                    UpdatedBy = d.UpdatedBy
                })
                .ToListAsync(cancellationToken);
        }
    }
}