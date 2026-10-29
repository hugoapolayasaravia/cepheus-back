using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.UpdateRegimenLaboral
{
    public class UpdateRegimenLaboralCommandHandler : IRequestHandler<UpdateRegimenLaboralCommand, RegimenLaboralResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateRegimenLaboralCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RegimenLaboralResponse> Handle(UpdateRegimenLaboralCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.RegimenesLaborales.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Régimen laboral {request.Code} no encontrado.");
            }

            var regimenLaboral = new RegimenLaboral
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.RegimenesLaborales.Update(regimenLaboral);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El régimen laboral fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new RegimenLaboralResponse
            {
                Code = regimenLaboral.Code,
                Name = regimenLaboral.Name,
                IsActive = regimenLaboral.IsActive,
                CreatedAt = regimenLaboral.CreatedAt,
                UpdatedAt = regimenLaboral.UpdatedAt,
                RowVersion = regimenLaboral.RowVersion
            };
        }
    }
}