using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.DeleteTrabajadorContrato
{
    public class DeleteTrabajadorContratoCommandHandler : IRequestHandler<DeleteTrabajadorContratoCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorContratoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorContratoCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorContratos.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorContrato {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorContratos.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
