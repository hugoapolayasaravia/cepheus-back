using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.UpdateOcupacion
{
    public class UpdateOcupacionCommandHandler : IRequestHandler<UpdateOcupacionCommand, OcupacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOcupacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OcupacionResponse> Handle(UpdateOcupacionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Ocupaciones.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Ocupación {request.Code} no encontrada.");
            }

            var ocupacion = new Ocupacion
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Ocupaciones.Update(ocupacion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La ocupación fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new OcupacionResponse
            {
                Code = ocupacion.Code,
                Name = ocupacion.Name,
                IsActive = ocupacion.IsActive,
                CreatedAt = ocupacion.CreatedAt,
                UpdatedAt = ocupacion.UpdatedAt,
                RowVersion = ocupacion.RowVersion
            };
        }
    }
}