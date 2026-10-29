using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.UpdateTrabajadorDomicilio
{
    public class UpdateTrabajadorDomicilioCommandHandler
        : IRequestHandler<UpdateTrabajadorDomicilioCommand, TrabajadorDomicilioResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorDomicilioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorDomicilioResponse> Handle(UpdateTrabajadorDomicilioCommand request, CancellationToken cancellationToken)
        {
            var domicilio = await _uow.Rrhh.Maestros.TrabajadorDomicilios.GetByIdAsync(request.Id, cancellationToken);

            if (domicilio is null)
            {
                throw new KeyNotFoundException($"Domicilio {request.Id} no encontrado.");
            }

            if (request.IsPrimary && !domicilio.IsPrimary)
            {
                var otros = await _uow.Rrhh.Maestros.TrabajadorDomicilios.Query()
                    .Where(d => d.TrabajadorCode == domicilio.TrabajadorCode && d.IsPrimary && d.Id != domicilio.Id)
                    .ToListAsync(cancellationToken);

                foreach (var otro in otros)
                {
                    otro.IsPrimary = false;
                }
            }

            domicilio.RoadTypeCode = string.IsNullOrWhiteSpace(request.RoadTypeCode) ? null : request.RoadTypeCode.Trim().ToUpperInvariant();
            domicilio.StreetName = string.IsNullOrWhiteSpace(request.StreetName) ? null : request.StreetName.Trim();
            domicilio.StreetNumber = string.IsNullOrWhiteSpace(request.StreetNumber) ? null : request.StreetNumber.Trim();
            domicilio.InteriorNumber = string.IsNullOrWhiteSpace(request.InteriorNumber) ? null : request.InteriorNumber.Trim();
            domicilio.ZoneTypeCode = string.IsNullOrWhiteSpace(request.ZoneTypeCode) ? null : request.ZoneTypeCode.Trim().ToUpperInvariant();
            domicilio.ZoneName = string.IsNullOrWhiteSpace(request.ZoneName) ? null : request.ZoneName.Trim();
            domicilio.Reference = string.IsNullOrWhiteSpace(request.Reference) ? null : request.Reference.Trim();
            domicilio.UbigeoCode = string.IsNullOrWhiteSpace(request.UbigeoCode) ? null : request.UbigeoCode.Trim().ToUpperInvariant();
            domicilio.IsPrimary = request.IsPrimary;

            await _uow.SaveChangesAsync(cancellationToken);

            return CreateTrabajadorDomicilio.CreateTrabajadorDomicilioCommandHandler.Map(domicilio);
        }
    }
}