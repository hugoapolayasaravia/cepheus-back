using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.UpdateTrabajadorDomicilio
{
    public record UpdateTrabajadorDomicilioCommand(
        int Id,
        string? RoadTypeCode,
        string? StreetName,
        string? StreetNumber,
        string? InteriorNumber,
        string? ZoneTypeCode,
        string? ZoneName,
        string? Reference,
        string? UbigeoCode,
        bool IsPrimary
    ) : IRequest<TrabajadorDomicilioResponse>;
}