using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.UpdateFalla
{
    public class UpdateFallaCommandHandler : IRequestHandler<UpdateFallaCommand, FallaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateFallaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FallaResponse> Handle(UpdateFallaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Mantenimiento.Catalogos.Fallas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Falla {request.Code} no encontrada.");
            }

            var falla = new Falla
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Mantenimiento.Catalogos.Fallas.Update(falla);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La falla fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new FallaResponse
            {
                Code = falla.Code,
                Name = falla.Name,
                IsActive = falla.IsActive,
                CreatedAt = falla.CreatedAt,
                UpdatedAt = falla.UpdatedAt,
                CreatedBy = falla.CreatedBy,
                UpdatedBy = falla.UpdatedBy,
                RowVersion = falla.RowVersion
            };
        }
    }
}
