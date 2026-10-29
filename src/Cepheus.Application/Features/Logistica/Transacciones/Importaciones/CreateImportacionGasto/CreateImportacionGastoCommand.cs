// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/CreateImportacionGasto/CreateImportacionGastoCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.CreateImportacionGasto
{
    /// <summary>
    /// TipoCambio, IgvExterior y Total NO se envían: los calcula el servidor.
    /// Igv es opcional: si no viene se calcula (NetoGasto × IGV%); si viene (y el comprobante no es del exterior) se respeta.
    /// MonedaCode: ISO (PEN = legacy 'S', USD = legacy 'D').
    /// </summary>
    public record CreateImportacionGastoCommand(
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
        decimal? Igv
    ) : IRequest<ImportacionResponse>;
}
