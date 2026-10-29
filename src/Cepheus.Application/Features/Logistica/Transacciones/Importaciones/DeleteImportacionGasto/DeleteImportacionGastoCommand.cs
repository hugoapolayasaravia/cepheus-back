// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/DeleteImportacionGasto/DeleteImportacionGastoCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.DeleteImportacionGasto
{
    public record DeleteImportacionGastoCommand(
        string PlantaCode,
        string ImportacionCode,
        string ProveedorCode,
        string NumeroDocumento
    ) : IRequest<ImportacionResponse>;
}
