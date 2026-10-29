using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.DeleteDetraccionItem
{
    /// <summary>Quita un ítem (proveedor + operación + bien) de un lote Pendiente y recalcula el total.</summary>
    public record DeleteDetraccionItemCommand(
        string Year,
        string Period,
        string LotNumber,
        string ProveedorCode,
        int ItemNumber
    ) : IRequest<DetraccionResponse>;
}
