using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.UpdateRegimenPensionario
{
    public class UpdateRegimenPensionarioCommandHandler
        : IRequestHandler<UpdateRegimenPensionarioCommand, RegimenPensionarioResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateRegimenPensionarioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RegimenPensionarioResponse> Handle(UpdateRegimenPensionarioCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.RegimenesPensionarios.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Régimen pensionario {request.Code} no encontrado.");
            }

            var regimenPensionario = new RegimenPensionario
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.RegimenesPensionarios.Update(regimenPensionario);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El régimen pensionario fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new RegimenPensionarioResponse
            {
                Code = regimenPensionario.Code,
                Name = regimenPensionario.Name,
                IsActive = regimenPensionario.IsActive,
                CreatedAt = regimenPensionario.CreatedAt,
                UpdatedAt = regimenPensionario.UpdatedAt,
                RowVersion = regimenPensionario.RowVersion
            };
        }
    }
}