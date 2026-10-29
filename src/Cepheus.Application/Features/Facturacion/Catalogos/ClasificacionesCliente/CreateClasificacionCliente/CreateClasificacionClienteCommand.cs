using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.CreateClasificacionCliente
{
    public record CreateClasificacionClienteCommand(
        string Name
    ) : IRequest<ClasificacionClienteResponse>;
}
