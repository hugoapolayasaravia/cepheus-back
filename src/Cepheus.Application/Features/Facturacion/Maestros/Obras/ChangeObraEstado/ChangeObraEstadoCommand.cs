using Cepheus.Application.Features.Facturacion.Maestros.Obras.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.ChangeObraEstado
{
    public record ChangeObraEstadoCommand(
        string ClienteCode,
        string Code,
        string NuevoEstado
    ) : IRequest<ObraResponse>;
}
