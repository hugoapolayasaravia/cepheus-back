using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.DeleteTrabajadorVacacion
{
    public class DeleteTrabajadorVacacionCommandHandler : IRequestHandler<DeleteTrabajadorVacacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorVacacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorVacacionCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorVacacions.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorVacacion {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorVacacions.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
