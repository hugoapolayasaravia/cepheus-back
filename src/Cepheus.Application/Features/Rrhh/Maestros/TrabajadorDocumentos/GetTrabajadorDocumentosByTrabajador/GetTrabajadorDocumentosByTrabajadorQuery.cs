using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.GetTrabajadorDocumentosByTrabajador
{
    public record GetTrabajadorDocumentosByTrabajadorQuery(string TrabajadorCode) : IRequest<List<TrabajadorDocumentoResponse>>;
}