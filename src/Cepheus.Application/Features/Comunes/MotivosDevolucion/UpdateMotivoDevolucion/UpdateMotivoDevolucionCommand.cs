using Cepheus.Application.Features.Comunes.MotivosDevolucion.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.UpdateMotivoDevolucion
{
    public record UpdateMotivoDevolucionCommand(
        string Code,
        string Name,
        bool AffectsStock,
        bool EsVenta,
        byte[] RowVersion
    ) : IRequest<MotivoDevolucionResponse>;
}
