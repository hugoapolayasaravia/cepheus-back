using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.UpdateClasificacionCliente
{
    public record UpdateClasificacionClienteCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<ClasificacionClienteResponse>;
}
