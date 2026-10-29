using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Areas.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Areas.UpdateArea
{
    public class UpdateAreaCommandHandler : IRequestHandler<UpdateAreaCommand, AreaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAreaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AreaResponse> Handle(UpdateAreaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Areas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Área {request.Code} no encontrada.");
            }

            var area = new Area
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Areas.Update(area);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El área fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new AreaResponse
            {
                Code = area.Code,
                Name = area.Name,
                IsActive = area.IsActive,
                CreatedAt = area.CreatedAt,
                UpdatedAt = area.UpdatedAt,
                RowVersion = area.RowVersion
            };
        }
    }
}