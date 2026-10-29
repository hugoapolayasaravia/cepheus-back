using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorJornadas.DeleteTrabajadorJornada
{
    public class DeleteTrabajadorJornadaCommandHandler : IRequestHandler<DeleteTrabajadorJornadaCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorJornadaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorJornadaCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorJornadas.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorContrato {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorJornadas.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
