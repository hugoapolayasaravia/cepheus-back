using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.CreateNoAfectoRetencion
{
    public record CreateNoAfectoRetencionCommand(
        string Ruc,
        string Type,
        string Name,
        string Resolution
    ) : IRequest<NoAfectoRetencionResponse>;
}
