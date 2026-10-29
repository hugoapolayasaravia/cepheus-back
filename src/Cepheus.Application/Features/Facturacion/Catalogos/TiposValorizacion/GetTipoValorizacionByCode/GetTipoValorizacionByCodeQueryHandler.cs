using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.GetTipoValorizacionByCode
{
    public class GetTipoValorizacionByCodeQueryHandler : IRequestHandler<GetTipoValorizacionByCodeQuery, TipoValorizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetTipoValorizacionByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoValorizacionResponse> Handle(GetTipoValorizacionByCodeQuery request, CancellationToken cancellationToken)
        {
            var tipoValorizacion = await _uow.Facturacion.Catalogos.TiposValorizacion.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new TipoValorizacionResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    Days = t.Days,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tipoValorizacion is null)
            {
                throw new KeyNotFoundException($"Tipo de valorización {request.Code} no encontrado.");
            }

            return tipoValorizacion;
        }
    }
}
