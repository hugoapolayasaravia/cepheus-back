using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.UpdateOportunidad
{
    public class UpdateOportunidadCommandHandler : IRequestHandler<UpdateOportunidadCommand, OportunidadResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateOportunidadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OportunidadResponse> Handle(UpdateOportunidadCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Catalogos.Oportunidades.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Oportunidad {request.Code} no encontrada.");
            }

            var oportunidad = new Oportunidad
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Catalogos.Oportunidades.Update(oportunidad);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La oportunidad fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new OportunidadResponse
            {
                Code = oportunidad.Code,
                Name = oportunidad.Name,
                IsActive = oportunidad.IsActive,
                CreatedAt = oportunidad.CreatedAt,
                UpdatedAt = oportunidad.UpdatedAt,
                RowVersion = oportunidad.RowVersion
            };
        }
    }
}
