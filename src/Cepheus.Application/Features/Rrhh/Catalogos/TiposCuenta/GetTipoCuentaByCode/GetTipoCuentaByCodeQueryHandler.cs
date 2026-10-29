using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCuenta.GetTipoCuentaByCode
{
    public class GetTipoCuentaByCodeQueryHandler : IRequestHandler<GetTipoCuentaByCodeQuery, TipoCuentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoCuentaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoCuentaResponse> Handle(GetTipoCuentaByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoCuenta = await _uow.Rrhh.Catalogos.TiposCuenta.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoCuentaResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoCuenta is null)
            {
                throw new KeyNotFoundException($"Tipo de cuenta {request.Code} no encontrado.");
            }

            return tipoCuenta;
        }
    }
}