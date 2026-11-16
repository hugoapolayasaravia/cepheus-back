using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Aprobaciones.GetResumenPendientes;

/// <summary>Cantidad de documentos pendientes por tipo de transacción. Todos = false: solo los que yo puedo aprobar.</summary>
public sealed record GetResumenPendientesQuery(bool Todos) : IRequest<List<ResumenPendientesItem>>;

public sealed class ResumenPendientesItem
{
    public string TipoTransaccion { get; set; } = default!;
    public string Nombre { get; set; } = default!;
    public int Cantidad { get; set; }
}
