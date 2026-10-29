using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValeOrdenTrabajoMateriales;

/// <summary>
/// Datos de la Orden de Trabajo para un vale (debe estar en ejecución) y sus materiales pendientes
/// (Logi_sp_Listado_TOTRMateriales). Con Todos = true trae los materiales en cualquier estado.
/// </summary>
public sealed record GetValeOrdenTrabajoMaterialesQuery(
    string PlantaCode,
    string OrdenTrabajoCode,
    bool Todos) : IRequest<ValeOrdenTrabajoMaterialesResponse>;
