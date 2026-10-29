using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.UpdateMotivoDevolucionArticulo
{
    public record UpdateMotivoDevolucionArticuloCommand(
        string Code,
        string Name,
        bool AffectsStock,
        bool EsVenta,
        byte[] RowVersion
    ) : IRequest<MotivoDevolucionArticuloResponse>;
}
