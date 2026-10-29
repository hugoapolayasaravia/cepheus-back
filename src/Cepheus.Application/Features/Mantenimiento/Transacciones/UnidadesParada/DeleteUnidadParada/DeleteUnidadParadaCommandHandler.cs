using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.DeleteUnidadParada
{
    public class DeleteUnidadParadaCommandHandler : IRequestHandler<DeleteUnidadParadaCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteUnidadParadaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteUnidadParadaCommand request, CancellationToken cancellationToken)
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var fecha = request.Fecha.Date;

            var entity = await _uow.Mantenimiento.Transacciones.UnidadesParada.Query()
                .FirstOrDefaultAsync(x => x.SubCentroCostoCode == scc && x.Fecha == fecha, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Registro {scc}/{fecha:yyyy-MM-dd} no encontrado.");
            }

            _uow.Mantenimiento.Transacciones.UnidadesParada.Remove(entity);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
