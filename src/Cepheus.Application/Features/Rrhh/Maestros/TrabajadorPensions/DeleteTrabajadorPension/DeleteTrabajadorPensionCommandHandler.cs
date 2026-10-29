using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.DeleteTrabajadorPension
{
    public class DeleteTrabajadorPensionCommandHandler : IRequestHandler<DeleteTrabajadorPensionCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorPensionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorPensionCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorPensions.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorPension {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorPensions.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
