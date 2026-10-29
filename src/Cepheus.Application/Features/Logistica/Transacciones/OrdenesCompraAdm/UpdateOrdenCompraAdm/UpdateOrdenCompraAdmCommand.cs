// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/UpdateOrdenCompraAdministracion/UpdateOrdenCompraAdministracionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.UpdateOrdenCompraAdm
{
    /// <summary>Actualiza solo la cabecera. Las líneas se administran con el detalle (siguiente entrega).</summary>
    public record UpdateOrdenCompraAdmCommand(
        string Code,
        string ComprobantePagoCode,
        string? EmitirDocumento,
        DateTime FechaEntrega,
        string ProveedorCode,
        string CompradorCode,
        string MonedaCode,
        string LugarEnvioCode,
        string FormaPagoCode,
        string TramiteCode,
        string? Observaciones1,
        string? Observaciones2,
        string? NotaCompraCode,
        string UnidadNegocioCode,
        string Motivo,
        decimal NoGravableCompra,
        decimal ServicioCompra,
        decimal IgvExteriorCompra,
        byte[] RowVersion
    ) : IRequest<OrdenCompraAdmResponse>, IOrdenCompraAdmCabecera;
}
