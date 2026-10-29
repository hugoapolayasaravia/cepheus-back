using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ClasificacionesCliente.GetClasificacionClienteByCode
{
    public class GetClasificacionClienteByCodeQueryHandler : IRequestHandler<GetClasificacionClienteByCodeQuery, ClasificacionClienteResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetClasificacionClienteByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ClasificacionClienteResponse> Handle(GetClasificacionClienteByCodeQuery request, CancellationToken cancellationToken)
        {
            var clasificacionCliente = await _uow.Facturacion.Catalogos.ClasificacionesCliente.Query()
                .AsNoTracking()
                .Where(t => t.Code == request.Code)
                .Select(t => new ClasificacionClienteResponse
                {
                    Code = t.Code,
                    Name = t.Name,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (clasificacionCliente is null)
            {
                throw new KeyNotFoundException($"Clasificación de cliente {request.Code} no encontrada.");
            }

            return clasificacionCliente;
        }
    }
}
