using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.DeleteTrabajadorBeneficio
{
    public class DeleteTrabajadorBeneficioCommandHandler : IRequestHandler<DeleteTrabajadorBeneficioCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteTrabajadorBeneficioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteTrabajadorBeneficioCommand request, CancellationToken cancellationToken)
        {
            var entidad = await _uow.Rrhh.Maestros.TrabajadorBeneficios.Query()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (entidad is null)
            {
                throw new KeyNotFoundException($"TrabajadorContrato {request.Id} no encontrado.");
            }

            _uow.Rrhh.Maestros.TrabajadorBeneficios.Remove(entidad);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
