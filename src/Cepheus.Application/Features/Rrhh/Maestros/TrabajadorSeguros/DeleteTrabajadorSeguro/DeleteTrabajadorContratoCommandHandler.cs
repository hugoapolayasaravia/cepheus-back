using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.DeleteTrabajadorSeguro
{
    public class DeleteTrabajadorSeguroCommandHandler : IRequestHandler<DeleteTrabajadorSeguroCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorSeguroCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorSeguroCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorSeguros.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorContrato {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorSeguros.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
