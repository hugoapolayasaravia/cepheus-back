using Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.Trabajadores.GetTrabajadorByCode
{
    public record GetTrabajadorByCodeQuery(string Code) : IRequest<TrabajadorResponse>;
}