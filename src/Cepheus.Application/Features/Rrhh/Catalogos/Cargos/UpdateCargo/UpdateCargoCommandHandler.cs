using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Cargos.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Cargos.UpdateCargo
{
    public class UpdateCargoCommandHandler : IRequestHandler<UpdateCargoCommand, CargoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCargoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CargoResponse> Handle(UpdateCargoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Cargos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Cargo {request.Code} no encontrado.");
            }

            var cargo = new Cargo
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Cargos.Update(cargo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El cargo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new CargoResponse
            {
                Code = cargo.Code,
                Name = cargo.Name,
                IsActive = cargo.IsActive,
                CreatedAt = cargo.CreatedAt,
                UpdatedAt = cargo.UpdatedAt,
                RowVersion = cargo.RowVersion
            };
        }
    }
}