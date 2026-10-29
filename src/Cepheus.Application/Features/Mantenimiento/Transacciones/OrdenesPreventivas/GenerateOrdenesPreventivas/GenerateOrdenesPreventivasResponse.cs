namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesPreventivas.GenerateOrdenesPreventivas;

public sealed record GenerateOrdenesPreventivasResponse(
    int TotalOrdenes,
    int TotalVales,
    IReadOnlyList<OrdenPreventivaGeneradaResponse> Ordenes);

public sealed record OrdenPreventivaGeneradaResponse(
    string PlantaCode,
    string PlanCode,
    string EquipoCode,
    string ActividadCode,
    string Gama,
    DateTime FechaOrden,
    string OrdenTrabajoCode,
    // null si el plan no tiene materiales (no se genera vale).
    string? ValeCode,
    int Materiales);
