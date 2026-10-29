using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.ResolverRangoAprobacion
{
    public class ResolverRangoAprobacionQueryHandler
        : IRequestHandler<ResolverRangoAprobacionQuery, ResolverRangoAprobacionResult>
    {
        private const int MaxNivelesJerarquia = 20; // guarda contra ciclos accidentales en UnidadNegocio.ParentCode

        private readonly IUnitOfWork _uow;

        public ResolverRangoAprobacionQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ResolverRangoAprobacionResult> Handle(
            ResolverRangoAprobacionQuery request, CancellationToken cancellationToken)
        {
            var trans = request.TipoTransaccionCode.Trim().ToUpperInvariant();
            var mon = request.MonedaCode.Trim().ToUpperInvariant();
            var uneActual = request.UnidadNegocioCode.Trim().ToUpperInvariant();

            for (var i = 0; i < MaxNivelesJerarquia; i++)
            {
                var rangos = await _uow.Logistica.Catalogos.RangosAprobacion.Query()
                    .AsNoTracking()
                    .Where(r => r.TipoTransaccionCode == trans && r.UnidadNegocioCode == uneActual && r.MonedaCode == mon)
                    .OrderBy(r => r.NivelCode)
                    .Select(r => new RangoAprobacionResponse
                    {
                        NivelCode = r.NivelCode,
                        TipoTransaccionCode = r.TipoTransaccionCode,
                        UnidadNegocioCode = r.UnidadNegocioCode,
                        MonedaCode = r.MonedaCode,
                        ImporteMinimo = r.ImporteMinimo,
                        ImporteMaximo = r.ImporteMaximo,
                        ImporteAcumuladoDiario = r.ImporteAcumuladoDiario,
                        ImporteAcumuladoMensual = r.ImporteAcumuladoMensual,
                        PorcentajeTotal = r.PorcentajeTotal,
                        IsActive = r.IsActive,
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        RowVersion = r.RowVersion
                    })
                    .ToListAsync(cancellationToken);

                if (rangos.Count > 0)
                {
                    var aplicable = rangos.FirstOrDefault(r =>
                        request.Monto >= r.ImporteMinimo && request.Monto <= r.ImporteMaximo);

                    return new ResolverRangoAprobacionResult
                    {
                        UnidadNegocioEfectivaCode = uneActual,
                        RangosDeLaUnidad = rangos,
                        RangoAplicable = aplicable
                    };
                }

                // No hay parametrización en esta unidad: subir al padre.
                var parentCode = await _uow.Logistica.Catalogos.UnidadesNegocio.Query()
                    .AsNoTracking()
                    .Where(u => u.Code == uneActual)
                    .Select(u => u.ParentCode)
                    .FirstOrDefaultAsync(cancellationToken);

                if (string.IsNullOrEmpty(parentCode))
                {
                    // Se llegó a la raíz sin encontrar parametrización.
                    break;
                }

                uneActual = parentCode;
            }

            throw new KeyNotFoundException(
                $"No se encontró parametrización de aprobación para la transacción {trans} en la unidad " +
                $"{request.UnidadNegocioCode} ni en ninguna de sus unidades superiores.");
        }
    }
}
