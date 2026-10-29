// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/CreateImportacionDetalle/CreateImportacionDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.CreateImportacionDetalle
{
    /// <summary>TipoCambio, ValorAduana y prorrateo NO se envían: los calcula el servidor.</summary>
    public record CreateImportacionDetalleCommand(
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
        decimal Seguro
    ) : IRequest<ImportacionResponse>;
}
