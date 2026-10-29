using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.DeleteTrabajadorSalud
{
    public class DeleteTrabajadorSaludCommandHandler : IRequestHandler<DeleteTrabajadorSaludCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorSaludCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorSaludCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorSaluds.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorSalud {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorSaluds.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
