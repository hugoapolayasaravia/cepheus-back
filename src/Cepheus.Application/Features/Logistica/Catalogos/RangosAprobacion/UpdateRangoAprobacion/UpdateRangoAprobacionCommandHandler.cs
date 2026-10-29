using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.UpdateRangoAprobacion
{
    public class UpdateRangoAprobacionCommandHandler
        : IRequestHandler<UpdateRangoAprobacionCommand, RangoAprobacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateRangoAprobacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RangoAprobacionResponse> Handle(
            UpdateRangoAprobacionCommand request, CancellationToken cancellationToken)
        {
            var nivel = request.NivelCode.Trim().ToUpperInvariant();
            var trans = request.TipoTransaccionCode.Trim().ToUpperInvariant();
            var une = request.UnidadNegocioCode.Trim().ToUpperInvariant();
            var mon = request.MonedaCode.Trim().ToUpperInvariant();

            var current = await _uow.Logistica.Catalogos.RangosAprobacion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.NivelCode == nivel && r.TipoTransaccionCode == trans &&
                    r.UnidadNegocioCode == une && r.MonedaCode == mon,
                    cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Rango de aprobación {nivel}/{trans}/{une}/{mon} no encontrado.");
            }

            var rango = new RangoAprobacion
            {
                NivelCode = nivel,
                TipoTransaccionCode = trans,
                UnidadNegocioCode = une,
                MonedaCode = mon,

                ImporteMinimo = request.ImporteMinimo,
                ImporteMaximo = request.ImporteMaximo,
                ImporteAcumuladoDiario = request.ImporteAcumuladoDiario,
                ImporteAcumuladoMensual = request.ImporteAcumuladoMensual,
                PorcentajeTotal = request.PorcentajeTotal,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,
                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.RangosAprobacion.Update(rango);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El rango de aprobación fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateRangoAprobacion.CreateRangoAprobacionCommandHandler.Map(rango);
        }
    }
}
