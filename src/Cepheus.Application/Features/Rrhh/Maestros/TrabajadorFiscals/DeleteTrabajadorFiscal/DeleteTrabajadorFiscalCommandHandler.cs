using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFiscals.DeleteTrabajadorFiscal
{
    public class DeleteTrabajadorFiscalCommandHandler : IRequestHandler<DeleteTrabajadorFiscalCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorFiscalCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorFiscalCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorFiscals.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorFiscal {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorFiscals.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
