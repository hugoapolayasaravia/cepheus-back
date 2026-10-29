using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.FormasPago.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.GetFormaPagoByCode
{
    public class GetFormaPagoByCodeQueryHandler : IRequestHandler<GetFormaPagoByCodeQuery, FormaPagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetFormaPagoByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FormaPagoResponse> Handle(GetFormaPagoByCodeQuery request, CancellationToken cancellationToken)
        {
            var formaPago = await _uow.Logistica.Catalogos.FormasPago.Query()
                .AsNoTracking()
                .Where(f => f.Code == request.Code)
                .Select(f => new FormaPagoResponse
                {
                    Code = f.Code,
                    Name = f.Name,
                    Days = f.Days,
                    IsCredit = f.IsCredit,
                    IsActive = f.IsActive,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    CreatedBy = f.CreatedBy,
                    UpdatedBy = f.UpdatedBy,
                    RowVersion = f.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (formaPago is null)
            {
                throw new KeyNotFoundException($"Forma de pago {request.Code} no encontrada.");
            }

            return formaPago;
        }
    }
}