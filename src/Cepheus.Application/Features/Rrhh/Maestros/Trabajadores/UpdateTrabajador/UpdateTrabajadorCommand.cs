using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.UpdateTrabajador
{
    public record UpdateTrabajadorCommand(
        string Code,
        string? FirstNames,
        string? PaternalSurname,
        string? MaternalSurname,
        string? SexoCode,
        string? EstadoCivilCode,
        string? NacionalidadCode,
        DateOnly? BirthDate,
        string? BirthUbigeoCode,
        string? Email,
        string? Phone,
        string? MobilePhone,
        string? PhotoUrl,
        bool HasDisability,
        byte[] RowVersion
    ) : IRequest<TrabajadorResponse>;
}