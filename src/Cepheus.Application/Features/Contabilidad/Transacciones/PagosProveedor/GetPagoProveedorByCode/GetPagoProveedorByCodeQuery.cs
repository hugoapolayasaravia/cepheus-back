using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.GetPagoProveedorByCode
{
    public record GetPagoProveedorByCodeQuery(string Code) : IRequest<PagoProveedorResponse>;
}
