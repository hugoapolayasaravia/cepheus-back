using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.DeleteTrabajadorContrato;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.DeleteTrabajadorContrato
{
    public class DeleteTrabajadorRemuneracionCommandHandler : IRequestHandler<DeleteTrabajadorRemuneracionCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorRemuneracionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorRemuneracionCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorRemuneracions.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorRemuneracion {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorRemuneracions.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
