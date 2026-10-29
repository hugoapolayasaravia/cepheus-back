using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.DeleteTrabajadorSindicato
{
    public class DeleteTrabajadorSindicatoCommandHandler : IRequestHandler<DeleteTrabajadorSindicatoCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorSindicatoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorSindicatoCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorSindicatos.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorSindicato {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorSindicatos.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
