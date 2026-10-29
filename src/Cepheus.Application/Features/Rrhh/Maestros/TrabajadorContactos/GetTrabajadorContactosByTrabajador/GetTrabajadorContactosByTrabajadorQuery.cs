using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.GetTrabajadorContactosByTrabajador
{
    public record GetTrabajadorContactosByTrabajadorQuery(string TrabajadorCode) : IRequest<List<TrabajadorContactoResponse>>;
}