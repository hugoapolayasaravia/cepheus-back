// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/RecalcularProrrateoImportacion/RecalcularProrrateoImportacionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.RecalcularProrrateoImportacion
{
    /// <summary>Equivale a Logi_sp_Actualiza_Importacion (el legacy lo ejecuta al Imprimir y al Generar).</summary>
    public record RecalcularProrrateoImportacionCommand(string PlantaCode, string ImportacionCode) : IRequest<ImportacionResponse>;
}
