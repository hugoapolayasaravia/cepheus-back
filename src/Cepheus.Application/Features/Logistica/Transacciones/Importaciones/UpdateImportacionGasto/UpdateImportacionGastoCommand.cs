// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/UpdateImportacionGasto/UpdateImportacionGastoCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacionGasto
{
    /// <summary>La clave (Proveedor + Número de documento) no se modifica.</summary>
    public record UpdateImportacionGastoCommand(
        string PlantaCode,
        string ImportacionCode,
        string ProveedorCode,
        string NumeroDocumento,
        string ComprobantePagoCode,
        string MonedaCode,
        bool Afecto,
        DateTime FechaEmision,
        decimal NetoGasto,
        decimal NetoGastoInafecto,
        decimal? Igv,
        byte[] RowVersion
    ) : IRequest<ImportacionResponse>;
}
