using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common;
using Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common.Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.NotasCompra.GetNotaCompraByCode
{
    public class GetNotaCompraByCodeQueryHandler : IRequestHandler<GetNotaCompraByCodeQuery, NotaCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNotaCompraByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCompraResponse> Handle(GetNotaCompraByCodeQuery request, CancellationToken cancellationToken)
        {
            var nota = await _uow.Logistica.Catalogos.NotasCompra.Query()
                .AsNoTracking()
                .Where(n => n.Code == request.Code)
                .Select(n => new NotaCompraResponse
                {
                    Code = n.Code,
                    Name = n.Name,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    CreatedBy = n.CreatedBy,
                    UpdatedBy = n.UpdatedBy,
                    RowVersion = n.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException($"Nota de compra {request.Code} no encontrada.");
            }

            return nota;
        }
    }
}