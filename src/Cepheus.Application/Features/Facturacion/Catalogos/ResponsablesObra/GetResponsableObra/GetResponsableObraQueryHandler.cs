using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.GetResponsableObra
{
    public class GetResponsableObraQueryHandler : IRequestHandler<GetResponsableObraQuery, ResponsableObraResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetResponsableObraQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ResponsableObraResponse> Handle(GetResponsableObraQuery request, CancellationToken cancellationToken)
        {
            var responsable = await _uow.Facturacion.Catalogos.ResponsablesObra.Query()
                .AsNoTracking()
                .Where(r =>
                    r.ClienteCode == request.ClienteCode &&
                    r.ObraCode == request.ObraCode &&
                    r.Code == request.Code)
                .Select(r => new ResponsableObraResponse
                {
                    ClienteCode = r.ClienteCode,
                    ObraCode = r.ObraCode,
                    Code = r.Code,
                    Name = r.Name,
                    Phone = r.Phone,
                    Position = r.Position,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    RowVersion = r.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (responsable is null)
            {
                throw new KeyNotFoundException("Responsable de obra no encontrado para la combinación indicada.");
            }

            return responsable;
        }
    }
}
