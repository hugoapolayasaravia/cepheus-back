using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.UpdateNoAfectoRetencion
{
    public record UpdateNoAfectoRetencionCommand(
        string Ruc,
        string Type,
        string Name,
        string Resolution,
        byte[] RowVersion
    ) : IRequest<NoAfectoRetencionResponse>;
}
