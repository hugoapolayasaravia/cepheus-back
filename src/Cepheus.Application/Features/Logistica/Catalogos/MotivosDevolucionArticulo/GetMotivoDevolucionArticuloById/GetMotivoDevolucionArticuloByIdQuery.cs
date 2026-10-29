using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.GetMotivoDevolucionArticuloById
{
    public record GetMotivoDevolucionArticuloByIdQuery(string Code) : IRequest<MotivoDevolucionArticuloResponse>;
}
