using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.GetTrabajadorContactoById
{
    public record GetTrabajadorContactoByIdQuery(int Id) : IRequest<TrabajadorContactoResponse>;
}