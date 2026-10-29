using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.CreateMotivoDevolucionArticulo
{
    public record CreateMotivoDevolucionArticuloCommand(
        string Code,
        string Name,
        bool AffectsStock,
        bool EsVenta
    ) : IRequest<MotivoDevolucionArticuloResponse>;
}
