using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.DeleteTrabajadorLaboral
{
    public class DeleteTrabajadorLaboralCommandHandler : IRequestHandler<DeleteTrabajadorLaboralCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorLaboralCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorLaboralCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorLaborals.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorLaboral {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorLaborals.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
