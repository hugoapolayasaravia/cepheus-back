using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.ComprobantesPago.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.GetComprobantePagoById
{
    public class GetComprobantePagoByIdQueryHandler : IRequestHandler<GetComprobantePagoByIdQuery, ComprobantePagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetComprobantePagoByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ComprobantePagoResponse> Handle(GetComprobantePagoByIdQuery request, CancellationToken cancellationToken)
        {
            var comprobante = await _uow.Comunes.ComprobantesPago.Query()
                .AsNoTracking()
                .Where(c => c.Code == request.Code)
                .Select(c => new ComprobantePagoResponse
                {
                    Code = c.Code,
                    SunatCode = c.SunatCode,
                    Name = c.Name,
                    ShortName = c.ShortName,
                    Description = c.Description,
                    RequiresRuc = c.RequiresRuc,
                    RequiresAddress = c.RequiresAddress,
                    AffectsIgv = c.AffectsIgv,
                    IsNonTaxable = c.IsNonTaxable,
                    AffectsIncomeTax = c.AffectsIncomeTax,
                    AffectsFonavi = c.AffectsFonavi,
                    IsService = c.IsService,
                    AffectsForeignIgv = c.AffectsForeignIgv,
                    AvailableForPurchaseOrder = c.AvailableForPurchaseOrder,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedBy = c.UpdatedBy,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (comprobante is null)
            {
                throw new KeyNotFoundException($"Comprobante de pago {request.Code} no encontrado.");
            }

            return comprobante;
        }
    }
}
