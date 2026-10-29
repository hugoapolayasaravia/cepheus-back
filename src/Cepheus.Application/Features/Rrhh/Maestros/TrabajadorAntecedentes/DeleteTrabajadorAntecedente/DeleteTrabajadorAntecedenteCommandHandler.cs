using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.DeleteTrabajadorAntecedente
{
    public class DeleteTrabajadorAntecedenteCommandHandler : IRequestHandler<DeleteTrabajadorAntecedenteCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorAntecedenteCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorAntecedenteCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorAntecedentes.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorAntecedente {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorAntecedentes.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
