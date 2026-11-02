using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.ToggleNoAfectoRetencionStatus
{
    public record ToggleNoAfectoRetencionStatusCommand(string Ruc, string Type) : IRequest<bool>;
}
