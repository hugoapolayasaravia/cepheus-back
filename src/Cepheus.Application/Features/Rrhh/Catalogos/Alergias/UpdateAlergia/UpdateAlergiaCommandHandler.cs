using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.Alergias.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Alergias.UpdateAlergia
{
    public class UpdateAlergiaCommandHandler : IRequestHandler<UpdateAlergiaCommand, AlergiaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAlergiaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AlergiaResponse> Handle(UpdateAlergiaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.Alergias.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Alergia {request.Code} no encontrada.");
            }

            var alergia = new Alergia
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.Alergias.Update(alergia);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La alergia fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new AlergiaResponse
            {
                Code = alergia.Code,
                Name = alergia.Name,
                IsActive = alergia.IsActive,
                CreatedAt = alergia.CreatedAt,
                UpdatedAt = alergia.UpdatedAt,
                RowVersion = alergia.RowVersion
            };
        }
    }
}