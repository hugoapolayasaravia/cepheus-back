using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposExtensionContrato.GetTipoExtensionContratoByCode
{
    public class GetTipoExtensionContratoByCodeQueryHandler
        : IRequestHandler<GetTipoExtensionContratoByCodeQuery, TipoExtensionContratoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoExtensionContratoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoExtensionContratoResponse> Handle(GetTipoExtensionContratoByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoExtensionContrato = await _uow.Rrhh.Catalogos.TiposExtensionContrato.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoExtensionContratoResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoExtensionContrato is null)
            {
                throw new KeyNotFoundException($"Tipo de extensión de contrato {request.Code} no encontrado.");
            }

            return tipoExtensionContrato;
        }
    }
}