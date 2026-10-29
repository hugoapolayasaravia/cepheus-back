using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDomicilios.DeleteTrabajadorDomicilio
{
    public class DeleteTrabajadorDomicilioCommandHandler : IRequestHandler<DeleteTrabajadorDomicilioCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorDomicilioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorDomicilioCommand request, CancellationToken cancellationToken)
        {
            var domicilio = await _uow.Rrhh.Maestros.TrabajadorDomicilios.GetByIdAsync(request.Id, cancellationToken);

            if (domicilio is null)
            {
                throw new KeyNotFoundException($"Domicilio {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorDomicilios.Remove(domicilio);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}