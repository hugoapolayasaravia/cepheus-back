// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/UpdateOrdenCompra/UpdateOrdenCompraCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.UpdateOrdenCompra
{
    public record UpdateOrdenCompraCommand(
        string PlantaCode,
        string Code,
        string TipoCompraCode,
        string ComprobantePagoCode,
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
        bool EnviarCorreoProveedor,
        string? MotivoRetraso,
        decimal NoGravableCompra,
        decimal ServicioCompra,
        decimal IgvExteriorCompra,
        byte[] RowVersion
    ) : IRequest<OrdenCompraResponse>;
}