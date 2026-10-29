using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.DeleteTrabajadorContable
{
    public class DeleteTrabajadorContableCommandHandler : IRequestHandler<DeleteTrabajadorContableCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorContableCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorContables.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorContable {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorContables.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
