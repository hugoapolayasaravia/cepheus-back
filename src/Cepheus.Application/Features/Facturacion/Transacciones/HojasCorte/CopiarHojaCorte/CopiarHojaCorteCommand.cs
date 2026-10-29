using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.CopiarHojaCorte;

/// <summary>
/// Copia una Hoja de Corte (de cualquier estado). La copia nace Pendiente, con el año/correlativo y la fecha de hoy,
/// y guarda la hoja de origen en ParentYear/ParentCode. Se copia cabecera y todas las líneas.
/// </summary>
public sealed record CopiarHojaCorteCommand(string Year, string Code) : IRequest<HojaCorteResponse>;
