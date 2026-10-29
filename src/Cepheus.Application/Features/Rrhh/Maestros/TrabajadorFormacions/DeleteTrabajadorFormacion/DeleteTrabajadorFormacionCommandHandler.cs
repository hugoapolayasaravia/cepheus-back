using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.DeleteTrabajadorFormacion
{
    public class DeleteTrabajadorFormacionCommandHandler : IRequestHandler<DeleteTrabajadorFormacionCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorFormacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorFormacionCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorFormacions.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorFormacion {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorFormacions.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
