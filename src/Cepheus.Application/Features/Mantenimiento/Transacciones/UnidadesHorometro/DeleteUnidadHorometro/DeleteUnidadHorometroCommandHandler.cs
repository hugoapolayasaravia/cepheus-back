using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.DeleteUnidadHorometro
{
    public class DeleteUnidadHorometroCommandHandler : IRequestHandler<DeleteUnidadHorometroCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteUnidadHorometroCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteUnidadHorometroCommand request, CancellationToken cancellationToken)
        {
            var scc = request.SubCentroCostoCode.Trim().ToUpperInvariant();
            var fecha = request.Fecha.Date;

            var entity = await _uow.Mantenimiento.Transacciones.UnidadesHorometro.Query()
                .FirstOrDefaultAsync(x => x.SubCentroCostoCode == scc && x.Fecha == fecha, cancellationToken);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Registro {scc}/{fecha:yyyy-MM-dd} no encontrado.");
            }

            _uow.Mantenimiento.Transacciones.UnidadesHorometro.Remove(entity);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
