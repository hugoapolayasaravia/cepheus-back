using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.GetHojaCorteByCode;

public sealed record GetHojaCorteByCodeQuery(string Year, string Code) : IRequest<HojaCorteResponse>;
