using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.GetNotaCotizacionPlantillaByCode
{
    public class GetNotaCotizacionPlantillaByCodeQueryHandler
        : IRequestHandler<GetNotaCotizacionPlantillaByCodeQuery, NotaCotizacionPlantillaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetNotaCotizacionPlantillaByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCotizacionPlantillaResponse> Handle(GetNotaCotizacionPlantillaByCodeQuery request, CancellationToken cancellationToken)
        {
            var nota = await _uow.Facturacion.Catalogos.NotasCotizacionPlantilla.Query()
                .AsNoTracking()
                .Where(t => t.NegocioCode == request.NegocioCode && t.Code == request.Code)
                .Select(t => new NotaCotizacionPlantillaResponse
                {
                    NegocioCode = t.NegocioCode,
                    Code = t.Code,
                    Description = t.Description,
                    Option = t.Option,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt,
                    RowVersion = t.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException($"Nota de cotización {request.NegocioCode}-{request.Code} no encontrada.");
            }

            return nota;
        }
    }
}
