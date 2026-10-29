// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/CreateImportacion/CreateImportacionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.CreateImportacion
{
    /// <summary>
    /// Alta de la cabecera de Importación. Los totales, el IGV y el tipo de
    /// cambio NO se envían: los calcula el servidor.
    /// </summary>
    public record CreateImportacionCommand(
        string PlantaCode,
        decimal PesoNeto,
        decimal PesoBruto,
        DateTime FechaPoliza,
        DateTime? FechaEntrega,
        decimal Advalorem,
        decimal Sobretasa,
        decimal OtrosGastos
    ) : IRequest<ImportacionResponse>;
}
