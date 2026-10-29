// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/UpdateImportacion/UpdateImportacionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.UpdateImportacion
{
    public record UpdateImportacionCommand(
        string PlantaCode,
        string Code,
        decimal PesoNeto,
        decimal PesoBruto,
        DateTime FechaPoliza,
        DateTime? FechaEntrega,
        decimal Advalorem,
        decimal Sobretasa,
        decimal OtrosGastos,
        byte[] RowVersion
    ) : IRequest<ImportacionResponse>;
}
