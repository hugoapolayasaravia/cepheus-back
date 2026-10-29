using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.CreateTrabajadorDomicilio
{
    public class CreateTrabajadorDomicilioCommandHandler
        : IRequestHandler<CreateTrabajadorDomicilioCommand, TrabajadorDomicilioResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorDomicilioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorDomicilioResponse> Handle(CreateTrabajadorDomicilioCommand request, CancellationToken cancellationToken)
        {
            var trabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant();

            if (request.IsPrimary)
            {
                var otros = await _uow.Rrhh.Maestros.TrabajadorDomicilios.Query()
                    .Where(d => d.TrabajadorCode == trabajadorCode && d.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var otro in otros)
                {
                    otro.IsPrimary = false;
                }
            }

            var domicilio = new TrabajadorDomicilio
            {
                TrabajadorCode = trabajadorCode,
                RoadTypeCode = string.IsNullOrWhiteSpace(request.RoadTypeCode) ? null : request.RoadTypeCode.Trim().ToUpperInvariant(),
                StreetName = string.IsNullOrWhiteSpace(request.StreetName) ? null : request.StreetName.Trim(),
                StreetNumber = string.IsNullOrWhiteSpace(request.StreetNumber) ? null : request.StreetNumber.Trim(),
                InteriorNumber = string.IsNullOrWhiteSpace(request.InteriorNumber) ? null : request.InteriorNumber.Trim(),
                ZoneTypeCode = string.IsNullOrWhiteSpace(request.ZoneTypeCode) ? null : request.ZoneTypeCode.Trim().ToUpperInvariant(),
                ZoneName = string.IsNullOrWhiteSpace(request.ZoneName) ? null : request.ZoneName.Trim(),
                Reference = string.IsNullOrWhiteSpace(request.Reference) ? null : request.Reference.Trim(),
                UbigeoCode = string.IsNullOrWhiteSpace(request.UbigeoCode) ? null : request.UbigeoCode.Trim().ToUpperInvariant(),
                IsPrimary = request.IsPrimary
            };

            await _uow.Rrhh.Maestros.TrabajadorDomicilios.AddAsync(domicilio, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(domicilio);
        }

        internal static TrabajadorDomicilioResponse Map(TrabajadorDomicilio domicilio) => new()
        {
            Id = domicilio.Id,
            TrabajadorCode = domicilio.TrabajadorCode,
            RoadTypeCode = domicilio.RoadTypeCode,
            StreetName = domicilio.StreetName,
            StreetNumber = domicilio.StreetNumber,
            InteriorNumber = domicilio.InteriorNumber,
            ZoneTypeCode = domicilio.ZoneTypeCode,
            ZoneName = domicilio.ZoneName,
            Reference = domicilio.Reference,
            UbigeoCode = domicilio.UbigeoCode,
            IsPrimary = domicilio.IsPrimary,
            CreatedAt = domicilio.CreatedAt,
            UpdatedAt = domicilio.UpdatedAt
        };
    }
}