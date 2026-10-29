using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.DeleteTrabajadorContacto
{
    public class DeleteTrabajadorContactoCommandHandler : IRequestHandler<DeleteTrabajadorContactoCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorContactoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorContactoCommand request, CancellationToken cancellationToken)
        {
            var contacto = await _uow.Rrhh.Maestros.TrabajadorContactos.GetByIdAsync(request.Id, cancellationToken);

            if (contacto is null)
            {
                throw new KeyNotFoundException($"Contacto {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorContactos.Remove(contacto);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}