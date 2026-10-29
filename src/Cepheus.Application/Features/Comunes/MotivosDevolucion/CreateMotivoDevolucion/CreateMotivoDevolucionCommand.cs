using Cepheus.Application.Features.Comunes.MotivosDevolucion.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.CreateMotivoDevolucion
{
    public record CreateMotivoDevolucionCommand(
        string Code,
        string Name,
        bool AffectsStock,
        bool EsVenta
    ) : IRequest<MotivoDevolucionResponse>;
}
