using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.UpdateFamilia
{
    public class UpdateFamiliaCommandHandler : IRequestHandler<UpdateFamiliaCommand, FamiliaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateFamiliaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FamiliaResponse> Handle(UpdateFamiliaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.Familias.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Familia {request.Code} no encontrada.");
            }

            var familia = new Familia
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.Familias.Update(familia);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La familia fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new FamiliaResponse
            {
                Code = familia.Code,
                Name = familia.Name,
                IsActive = familia.IsActive,
                CreatedAt = familia.CreatedAt,
                UpdatedAt = familia.UpdatedAt,
                CreatedBy = familia.CreatedBy,
                UpdatedBy = familia.UpdatedBy,
                RowVersion = familia.RowVersion
            };
        }
    }
}