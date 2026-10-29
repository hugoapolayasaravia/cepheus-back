using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.GetCierreIncidencias;

public sealed class GetCierreIncidenciasQueryHandler
    : IRequestHandler<GetCierreIncidenciasQuery, IReadOnlyList<CierreIncidenciaResponse>>
{
    private readonly IUnitOfWork _uow;

    public GetCierreIncidenciasQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<CierreIncidenciaResponse>> Handle(
        GetCierreIncidenciasQuery request, CancellationToken ct)
    {
        var planta = request.PlantaCode.Trim().ToUpperInvariant();
        var periodo = request.PeriodCode.Trim();

        if (!ControlCierreRules.IsValidPeriod(periodo))
            throw new InvalidOperationException("El período debe tener el formato AAAAMM.");

        var query = _uow.Logistica.Transacciones.CierreIncidencias.Query().AsNoTracking()
            .Where(i => i.PlantaCode == planta && i.PeriodCode == periodo);

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            if (!Enum.TryParse<TipoIncidenciaCierre>(request.Tipo.Trim(), ignoreCase: true, out var tipo))
                throw new InvalidOperationException(
                    "Tipo de incidencia inválido. Valores: " + string.Join(", ", Enum.GetNames<TipoIncidenciaCierre>()));

            query = query.Where(i => i.Tipo == tipo);
        }

        var filas = await query
            .OrderBy(i => i.Tipo).ThenBy(i => i.ArticuloCode)
            .Select(i => new { i.Id, i.ArticuloCode, ArticuloName = i.Articulo.Name, i.Tipo, i.DocumentoCode, i.Mensaje })
            .ToListAsync(ct);

        return filas
            .Select(f => new CierreIncidenciaResponse(
                f.Id, f.ArticuloCode, f.ArticuloName, f.Tipo.ToString(), f.DocumentoCode, f.Mensaje))
            .ToList();
    }
}
