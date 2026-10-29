using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.GetTrabajadorFiscalsByTrabajador
{
    public record GetTrabajadorFiscalsByTrabajadorQuery(string TrabajadorCode) : IRequest<TrabajadorFiscalResponse?>;
}
