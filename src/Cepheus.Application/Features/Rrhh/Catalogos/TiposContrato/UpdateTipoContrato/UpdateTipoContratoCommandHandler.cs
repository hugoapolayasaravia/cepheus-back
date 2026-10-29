using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.UpdateTipoContrato
{
    public class UpdateTipoContratoCommandHandler : IRequestHandler<UpdateTipoContratoCommand, TipoContratoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoContratoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoContratoResponse> Handle(UpdateTipoContratoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposContrato.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de contrato {request.Code} no encontrado.");
            }

            var tipoContrato = new TipoContrato
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposContrato.Update(tipoContrato);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de contrato fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoContratoResponse
            {
                Code = tipoContrato.Code,
                Name = tipoContrato.Name,
                IsActive = tipoContrato.IsActive,
                CreatedAt = tipoContrato.CreatedAt,
                UpdatedAt = tipoContrato.UpdatedAt,
                RowVersion = tipoContrato.RowVersion
            };
        }
    }
}