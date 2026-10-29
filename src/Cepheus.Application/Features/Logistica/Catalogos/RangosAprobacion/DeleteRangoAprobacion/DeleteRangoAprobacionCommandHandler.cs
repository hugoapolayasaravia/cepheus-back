using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.DeleteRangoAprobacion
{
    public class DeleteRangoAprobacionCommandHandler : IRequestHandler<DeleteRangoAprobacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteRangoAprobacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteRangoAprobacionCommand request, CancellationToken cancellationToken)
        {
            var nivel = request.NivelCode.Trim().ToUpperInvariant();
            var trans = request.TipoTransaccionCode.Trim().ToUpperInvariant();
            var une = request.UnidadNegocioCode.Trim().ToUpperInvariant();
            var mon = request.MonedaCode.Trim().ToUpperInvariant();

            var rango = await _uow.Logistica.Catalogos.RangosAprobacion.Query()
                .FirstOrDefaultAsync(r =>
                    r.NivelCode == nivel && r.TipoTransaccionCode == trans &&
                    r.UnidadNegocioCode == une && r.MonedaCode == mon,
                    cancellationToken);

            if (rango is null)
            {
                throw new KeyNotFoundException($"Rango de aprobación {nivel}/{trans}/{une}/{mon} no encontrado.");
            }

            _uow.Logistica.Catalogos.RangosAprobacion.Remove(rango);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
