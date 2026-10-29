using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.AnularHojaCorte;

/// <summary>Anula la Hoja de Corte (solo Pendiente y sin Orden de Trabajo asociada).</summary>
public sealed record AnularHojaCorteCommand(string Year, string Code) : IRequest<HojaCorteResponse>;
