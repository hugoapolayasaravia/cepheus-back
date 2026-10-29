using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.UpdateTipoPension
{
    public class UpdateTipoPensionCommandHandler : IRequestHandler<UpdateTipoPensionCommand, TipoPensionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoPensionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoPensionResponse> Handle(UpdateTipoPensionCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposPension.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de pensión {request.Code} no encontrado.");
            }

            var tipoPension = new TipoPension
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposPension.Update(tipoPension);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de pensión fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoPensionResponse
            {
                Code = tipoPension.Code,
                Name = tipoPension.Name,
                IsActive = tipoPension.IsActive,
                CreatedAt = tipoPension.CreatedAt,
                UpdatedAt = tipoPension.UpdatedAt,
                RowVersion = tipoPension.RowVersion
            };
        }
    }
}