using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Catalogos.ModosPago.GetModoPagoByCode
{
    public class GetModoPagoByCodeQueryHandler : IRequestHandler<GetModoPagoByCodeQuery, ModoPagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetModoPagoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ModoPagoResponse> Handle(GetModoPagoByCodeQuery request, CancellationToken cancellationToken)
        {
            var modoPago = await _uow.Rrhh.Catalogos.ModosPago.Query()
                .AsNoTracking()
                .Where(m => m.Code == request.Code)
                .Select(m => new ModoPagoResponse
                {
                    Code = m.Code,
                    Name = m.Name,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    RowVersion = m.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (modoPago is null)
            {
                throw new KeyNotFoundException($"Modo de pago {request.Code} no encontrado.");
            }

            return modoPago;
        }
    }
}