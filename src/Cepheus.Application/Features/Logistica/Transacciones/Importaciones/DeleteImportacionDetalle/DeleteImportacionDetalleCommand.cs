// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/DeleteImportacionDetalle/DeleteImportacionDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.DeleteImportacionDetalle
{
    public record DeleteImportacionDetalleCommand(
        string PlantaCode,
        string ImportacionCode,
        string ProveedorCode,
        string ArticuloCode
    ) : IRequest<ImportacionResponse>;
}
