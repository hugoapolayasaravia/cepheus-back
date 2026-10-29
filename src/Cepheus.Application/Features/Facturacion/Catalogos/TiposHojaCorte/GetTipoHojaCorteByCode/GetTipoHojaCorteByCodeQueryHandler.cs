using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposHojaCorte.GetTipoHojaCorteByCode
{
    public class GetTipoHojaCorteByCodeQueryHandler : IRequestHandler<GetTipoHojaCorteByCodeQuery, TipoHojaCorteResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoHojaCorteByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoHojaCorteResponse> Handle(GetTipoHojaCorteByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoHojaCorte = await _uow.Facturacion.Catalogos.TiposHojaCorte.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoHojaCorteResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoHojaCorte is null)
            {
                throw new KeyNotFoundException($"Tipo de hoja de corte {request.Code} no encontrado.");
            }

            return tipoHojaCorte;
        }
    }
}
