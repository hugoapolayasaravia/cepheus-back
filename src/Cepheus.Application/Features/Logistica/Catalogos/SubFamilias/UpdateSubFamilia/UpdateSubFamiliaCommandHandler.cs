using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.UpdateSubFamilia
{
    public class UpdateSubFamiliaCommandHandler : IRequestHandler<UpdateSubFamiliaCommand, SubFamiliaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSubFamiliaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SubFamiliaResponse> Handle(UpdateSubFamiliaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Catalogos.SubFamilias.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"SubFamilia {request.Code} no encontrada.");
            }

            var subFamilia = new SubFamilia
            {
                Code = request.Code,
                FamiliaCode = current.FamiliaCode,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.SubFamilias.Update(subFamilia);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La subfamilia fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new SubFamiliaResponse
            {
                Code = subFamilia.Code,
                FamiliaCode = subFamilia.FamiliaCode,
                Name = subFamilia.Name,
                IsActive = subFamilia.IsActive,
                CreatedAt = subFamilia.CreatedAt,
                UpdatedAt = subFamilia.UpdatedAt,
                CreatedBy = subFamilia.CreatedBy,
                UpdatedBy = subFamilia.UpdatedBy,
                RowVersion = subFamilia.RowVersion
            };
        }
    }
}