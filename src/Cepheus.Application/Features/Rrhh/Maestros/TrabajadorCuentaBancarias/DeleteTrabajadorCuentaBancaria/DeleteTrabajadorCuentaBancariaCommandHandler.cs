using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorCuentaBancarias.DeleteTrabajadorCuentaBancaria
{
    public class DeleteTrabajadorCuentaBancariaCommandHandler : IRequestHandler<DeleteTrabajadorCuentaBancariaCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorCuentaBancariaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorCuentaBancariaCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorCuentaBancarias.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorCuentaBancaria {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorCuentaBancarias.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
