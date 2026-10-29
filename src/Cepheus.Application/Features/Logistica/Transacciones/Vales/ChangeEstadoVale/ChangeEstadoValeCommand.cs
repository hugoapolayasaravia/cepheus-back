using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ChangeEstadoVale;

/// <summary>
/// Aprobación del vale: Pendiente -> Aprobado, o Aprobado -> Pendiente (lo desaprueba quien lo aprobó).
/// Procesar, devolver y anular tienen sus propios comandos.
/// </summary>
public sealed record ChangeEstadoValeCommand(
    string PlantaCode,
    string Code,
    string NuevoEstado) : IRequest<ValeResponse>;
