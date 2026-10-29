// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/UpdateImportacionDetalle/UpdateImportacionDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacionDetalle
{
    /// <summary>La clave (Proveedor + Artículo) no se modifica (legacy: campos bloqueados al modificar).</summary>
    public record UpdateImportacionDetalleCommand(
        string PlantaCode,
        string ImportacionCode,
        string ProveedorCode,
        string ArticuloCode,
        string ComprobantePagoCode,
        string NumeroDocumento,
        DateTime FechaEmision,
        decimal Cantidad,
        decimal ValorFob,
        decimal Flete,
        decimal Seguro,
        byte[] RowVersion
    ) : IRequest<ImportacionResponse>;
}
