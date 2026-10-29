using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.CreateRangoAprobacion
{
    public class CreateRangoAprobacionCommandHandler
        : IRequestHandler<CreateRangoAprobacionCommand, RangoAprobacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateRangoAprobacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RangoAprobacionResponse> Handle(
            CreateRangoAprobacionCommand request, CancellationToken cancellationToken)
        {
            var rango = new RangoAprobacion
            {
                NivelCode = request.NivelCode.Trim().ToUpperInvariant(),
                TipoTransaccionCode = request.TipoTransaccionCode.Trim().ToUpperInvariant(),
                UnidadNegocioCode = request.UnidadNegocioCode.Trim().ToUpperInvariant(),
                MonedaCode = request.MonedaCode.Trim().ToUpperInvariant(),
                ImporteMinimo = request.ImporteMinimo,
                ImporteMaximo = request.ImporteMaximo,
                ImporteAcumuladoDiario = request.ImporteAcumuladoDiario,
                ImporteAcumuladoMensual = request.ImporteAcumuladoMensual,
                PorcentajeTotal = request.PorcentajeTotal,
                IsActive = true
            };

            await _uow.Logistica.Catalogos.RangosAprobacion.AddAsync(rango, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(rango);
        }

        internal static RangoAprobacionResponse Map(RangoAprobacion r) => new()
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
            CreatedBy = r.CreatedBy,
            UpdatedBy = r.UpdatedBy,
            RowVersion = r.RowVersion
        };
    }
}
