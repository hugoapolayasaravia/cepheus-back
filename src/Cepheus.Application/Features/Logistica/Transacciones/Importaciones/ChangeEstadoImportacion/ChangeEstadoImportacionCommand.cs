// src/Cepheus.Application/Features/Logistica/Transacciones/Importaciones/ChangeEstadoImportacion/ChangeEstadoImportacionCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Importaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Importaciones.ChangeEstadoImportacion
{
    public record ChangeEstadoImportacionCommand(string PlantaCode, string Code, string NuevoEstado)
        : IRequest<ImportacionResponse>;
}
