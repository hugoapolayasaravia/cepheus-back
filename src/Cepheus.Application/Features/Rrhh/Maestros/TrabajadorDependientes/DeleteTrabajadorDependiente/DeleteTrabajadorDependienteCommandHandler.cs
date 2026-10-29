using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.DeleteTrabajadorDependiente
{
    public class DeleteTrabajadorDependienteCommandHandler : IRequestHandler<DeleteTrabajadorDependienteCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorDependienteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorDependienteCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorDependientes.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorDependiente {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorDependientes.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
