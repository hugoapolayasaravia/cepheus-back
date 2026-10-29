using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.UpdateSubOcupacion
{
    public class UpdateSubOcupacionCommandHandler : IRequestHandler<UpdateSubOcupacionCommand, SubOcupacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSubOcupacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubOcupacionResponse> Handle(UpdateSubOcupacionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.SubOcupaciones.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Sub ocupación {request.Code} no encontrada.");
            }

            var subOcupacion = new SubOcupacion
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.SubOcupaciones.Update(subOcupacion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La sub ocupación fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new SubOcupacionResponse
            {
                Code = subOcupacion.Code,
                Name = subOcupacion.Name,
                IsActive = subOcupacion.IsActive,
                CreatedAt = subOcupacion.CreatedAt,
                UpdatedAt = subOcupacion.UpdatedAt,
                RowVersion = subOcupacion.RowVersion
            };
        }
    }
}