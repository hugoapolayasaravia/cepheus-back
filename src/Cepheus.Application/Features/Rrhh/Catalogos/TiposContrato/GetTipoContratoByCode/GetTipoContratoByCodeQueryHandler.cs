using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposContrato.GetTipoContratoByCode
{
    public class GetTipoContratoByCodeQueryHandler : IRequestHandler<GetTipoContratoByCodeQuery, TipoContratoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoContratoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoContratoResponse> Handle(GetTipoContratoByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoContrato = await _uow.Rrhh.Catalogos.TiposContrato.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoContratoResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoContrato is null)
            {
                throw new KeyNotFoundException($"Tipo de contrato {request.Code} no encontrado.");
            }

            return tipoContrato;
        }
    }
}