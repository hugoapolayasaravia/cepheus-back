using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.GetNoAfectoRetencionByKey
{
    public record GetNoAfectoRetencionByKeyQuery(string Ruc, string Type) : IRequest<NoAfectoRetencionResponse>;
}
