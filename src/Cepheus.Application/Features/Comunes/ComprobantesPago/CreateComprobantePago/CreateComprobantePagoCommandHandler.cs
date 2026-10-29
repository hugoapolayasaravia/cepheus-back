using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.ComprobantesPago.Common;
using Cepheus.Domain.Comunes;
using MediatR;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.CreateComprobantePago
{
    public class CreateComprobantePagoCommandHandler : IRequestHandler<CreateComprobantePagoCommand, ComprobantePagoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateComprobantePagoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ComprobantePagoResponse> Handle(CreateComprobantePagoCommand request, CancellationToken cancellationToken)
        {

            var code = await SequentialCodeGenerator.NextAsync(
            _uow.Comunes.ComprobantesPago.Query().Select(b => b.Code), length: 2, entityLabel: "Comprobantes", cancellationToken);


            var comprobante = new ComprobantePago
            {
                Code = code,
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
                IsActive = true
            };

            await _uow.Comunes.ComprobantesPago.AddAsync(comprobante, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(comprobante);
        }

        internal static ComprobantePagoResponse Map(ComprobantePago comprobante) => new()
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
