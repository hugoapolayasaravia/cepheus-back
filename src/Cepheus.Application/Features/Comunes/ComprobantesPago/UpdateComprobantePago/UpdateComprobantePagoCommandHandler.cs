using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.ComprobantesPago.Common;
using Cepheus.Domain.Comunes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.UpdateComprobantePago
{
    public class UpdateComprobantePagoCommandHandler : IRequestHandler<UpdateComprobantePagoCommand, ComprobantePagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateComprobantePagoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ComprobantePagoResponse> Handle(UpdateComprobantePagoCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Comunes.ComprobantesPago.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Comprobante de pago {request.Code} no encontrado.");
            }

            var comprobante = new ComprobantePago
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                SunatCode = request.SunatCode.Trim(),
                Name = request.Name.Trim(),
                ShortName = request.ShortName.Trim().ToUpperInvariant(),
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
                RequiresRuc = request.RequiresRuc,
                RequiresAddress = request.RequiresAddress,
                AffectsIgv = request.AffectsIgv,
                IsNonTaxable = request.IsNonTaxable,
                AffectsIncomeTax = request.AffectsIncomeTax,
                AffectsFonavi = request.AffectsFonavi,
                IsService = request.IsService,
                AffectsForeignIgv = request.AffectsForeignIgv,
                AvailableForPurchaseOrder = request.AvailableForPurchaseOrder,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Comunes.ComprobantesPago.Update(comprobante);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El comprobante de pago fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ComprobantePagoResponse
            {
                Code = comprobante.Code,
                SunatCode = comprobante.SunatCode,
                Name = comprobante.Name,
                ShortName = comprobante.ShortName,
                Description = comprobante.Description,
                RequiresRuc = comprobante.RequiresRuc,
                RequiresAddress = comprobante.RequiresAddress,
                AffectsIgv = comprobante.AffectsIgv,
                IsNonTaxable = comprobante.IsNonTaxable,
                AffectsIncomeTax = comprobante.AffectsIncomeTax,
                AffectsFonavi = comprobante.AffectsFonavi,
                IsService = comprobante.IsService,
                AffectsForeignIgv = comprobante.AffectsForeignIgv,
                AvailableForPurchaseOrder = comprobante.AvailableForPurchaseOrder,
                IsActive = comprobante.IsActive,
                CreatedAt = comprobante.CreatedAt,
                UpdatedAt = comprobante.UpdatedAt,
                CreatedBy = comprobante.CreatedBy,
                UpdatedBy = comprobante.UpdatedBy,
                RowVersion = comprobante.RowVersion
            };
        }
    }
}
