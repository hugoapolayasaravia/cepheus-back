using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.GetTipoTransaccionByCode
{
    public class GetTipoTransaccionByCodeQueryHandler
        : IRequestHandler<GetTipoTransaccionByCodeQuery, TipoTransaccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoTransaccionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoTransaccionResponse> Handle(
            GetTipoTransaccionByCodeQuery request,
            CancellationToken cancellationToken)
        {
            var tipoTransaccion = await _uow.Logistica.Catalogos.TiposTransaccion.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoTransaccionResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoTransaccion is null)
            {
                throw new KeyNotFoundException($"Tipo de transacción {request.Code} no encontrado.");
            }

            return tipoTransaccion;
        }
    }
}
