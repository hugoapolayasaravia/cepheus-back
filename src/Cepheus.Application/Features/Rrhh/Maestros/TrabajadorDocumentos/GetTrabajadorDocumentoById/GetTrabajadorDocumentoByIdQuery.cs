using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.GetTrabajadorDocumentoById
{
    public record GetTrabajadorDocumentoByIdQuery(int Id) : IRequest<TrabajadorDocumentoResponse>;
}