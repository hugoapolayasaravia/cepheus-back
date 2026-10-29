using Cepheus.Domain.Logistica.Enum;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.GetCierreIncidencias;

public sealed record GetCierreIncidenciasQuery(string PlantaCode, string PeriodCode, string? Tipo)
    : IRequest<IReadOnlyList<CierreIncidenciaResponse>>;

public sealed record CierreIncidenciaResponse(
    long Id,
    string ArticuloCode,
    string ArticuloName,
    string Tipo,
    string? DocumentoCode,
    string Mensaje);
