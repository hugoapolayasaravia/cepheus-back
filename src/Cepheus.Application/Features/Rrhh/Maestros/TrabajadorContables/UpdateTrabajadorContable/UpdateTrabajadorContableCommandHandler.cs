using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContables.UpdateTrabajadorContable
{
    public class UpdateTrabajadorContableCommandHandler : IRequestHandler<UpdateTrabajadorContableCommand, TrabajadorContableResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorContableCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorContableResponse> Handle(UpdateTrabajadorContableCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Maestros.TrabajadorContables.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"TrabajadorContable {request.Id} no encontrado.");
            }

            var entidad = new TrabajadorContable
            {
                Id = request.Id,
                TrabajadorCode = current.TrabajadorCode,
                NumeroItem = request.NumeroItem,
                CuentaContable = string.IsNullOrWhiteSpace(request.CuentaContable) ? null : request.CuentaContable!.Trim(),
                Tipo = string.IsNullOrWhiteSpace(request.Tipo) ? null : request.Tipo!.Trim(),
                Porcentaje = request.Porcentaje,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Maestros.TrabajadorContables.Update(entidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El registro fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateTrabajadorContable.CreateTrabajadorContableCommandHandler.Map(entidad);
        }
    }
}
