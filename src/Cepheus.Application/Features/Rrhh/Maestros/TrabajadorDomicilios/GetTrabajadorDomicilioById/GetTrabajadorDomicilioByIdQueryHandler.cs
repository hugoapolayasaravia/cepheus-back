using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.GetTrabajadorDomicilioById
{
    public class GetTrabajadorDomicilioByIdQueryHandler
        : IRequestHandler<GetTrabajadorDomicilioByIdQuery, TrabajadorDomicilioResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTrabajadorDomicilioByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorDomicilioResponse> Handle(GetTrabajadorDomicilioByIdQuery request, CancellationToken cancellationToken)
        {
            var domicilio = await _uow.Rrhh.Maestros.TrabajadorDomicilios.Query()
                .AsNoTracking()
                .Where(d => d.Id == request.Id)
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
                    UpdatedAt = d.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (domicilio is null)
            {
                throw new KeyNotFoundException($"Domicilio {request.Id} no encontrado.");
            }

            return domicilio;
        }
    }
}