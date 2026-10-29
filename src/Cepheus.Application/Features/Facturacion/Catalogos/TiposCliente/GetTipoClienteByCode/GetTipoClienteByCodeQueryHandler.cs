using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.GetTipoClienteByCode
{
    public class GetTipoClienteByCodeQueryHandler : IRequestHandler<GetTipoClienteByCodeQuery, TipoClienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoClienteByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoClienteResponse> Handle(GetTipoClienteByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoCliente = await _uow.Facturacion.Catalogos.TiposCliente.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoClienteResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoCliente is null)
            {
                throw new KeyNotFoundException($"Tipo de cliente {request.Code} no encontrado.");
            }

            return tipoCliente;
        }
    }
}
