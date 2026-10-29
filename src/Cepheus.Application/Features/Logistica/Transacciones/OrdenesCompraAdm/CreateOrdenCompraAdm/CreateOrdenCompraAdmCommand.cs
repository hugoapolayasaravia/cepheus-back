// Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdministracion/CreateOrdenCompraAdministracion/CreateOrdenCompraAdministracionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompraAdm.CreateOrdenCompraAdm
{
    /// <summary>Motivo: nombre del enum MotivoOrdenCompraAdministracion (Otros, Detracciones, Publicaciones, ...).</summary>
    public record CreateOrdenCompraAdmCommand(
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
        List<CreateOrdenCompraAdmDetalleInput> Detalles
    ) : IRequest<OrdenCompraAdmResponse>, IOrdenCompraAdmCabecera;

    public record CreateOrdenCompraAdmDetalleInput(
        string Glosa,
        string UnidadMedidaCode,
        decimal Cantidad,
        decimal Precio,
        decimal Descuento,
        string SubCentroCostoCode
    );
}
