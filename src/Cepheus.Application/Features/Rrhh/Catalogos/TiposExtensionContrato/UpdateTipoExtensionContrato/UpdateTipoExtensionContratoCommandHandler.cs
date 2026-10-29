using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.UpdateTipoExtensionContrato
{
    public class UpdateTipoExtensionContratoCommandHandler
        : IRequestHandler<UpdateTipoExtensionContratoCommand, TipoExtensionContratoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTipoExtensionContratoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoExtensionContratoResponse> Handle(UpdateTipoExtensionContratoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Rrhh.Catalogos.TiposExtensionContrato.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Tipo de extensión de contrato {request.Code} no encontrado.");
            }

            var tipoExtensionContrato = new TipoExtensionContrato
            {
                Code = request.Code,
                Name = request.Name.Trim(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Rrhh.Catalogos.TiposExtensionContrato.Update(tipoExtensionContrato);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El tipo de extensión de contrato fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new TipoExtensionContratoResponse
            {
                Code = tipoExtensionContrato.Code,
                Name = tipoExtensionContrato.Name,
                IsActive = tipoExtensionContrato.IsActive,
                CreatedAt = tipoExtensionContrato.CreatedAt,
                UpdatedAt = tipoExtensionContrato.UpdatedAt,
                RowVersion = tipoExtensionContrato.RowVersion
            };
        }
    }
}