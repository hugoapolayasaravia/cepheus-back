using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Proveedores.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.CreateProveedor
{
    public class CreateProveedorCommandHandler : IRequestHandler<CreateProveedorCommand, ProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorResponse> Handle(CreateProveedorCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Maestros.Proveedores.Query().Select(p => p.Code), length: 5, entityLabel: "Proveedores", cancellationToken);

            var proveedor = new Proveedor
            {
                Code = code,
                DocumentTypeCode = request.DocumentTypeCode.Trim().ToUpperInvariant(),
                DocumentNumber = request.DocumentNumber.Trim(),
                LegalName = request.LegalName.Trim(),
                TradeName = string.IsNullOrWhiteSpace(request.TradeName) ? null : request.TradeName.Trim(),
                ProviderType = request.ProviderType,
                Origin = request.Origin,
                SunatCondition = request.SunatCondition,
                SunatStatus = request.SunatStatus,
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),
                IsActive = true
            };

            await _uow.Logistica.Maestros.Proveedores.AddAsync(proveedor, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(proveedor);
        }

        internal static ProveedorResponse Map(Proveedor proveedor) => new()
        {
            Code = proveedor.Code,
            DocumentTypeCode = proveedor.DocumentTypeCode,
            DocumentNumber = proveedor.DocumentNumber,
            LegalName = proveedor.LegalName,
            TradeName = proveedor.TradeName,
            ProviderType = proveedor.ProviderType,
            Origin = proveedor.Origin,
            SunatCondition = proveedor.SunatCondition,
            SunatStatus = proveedor.SunatStatus,
            Observations = proveedor.Observations,
            IsActive = proveedor.IsActive,
            DeactivatedAt = proveedor.DeactivatedAt,
            CreatedAt = proveedor.CreatedAt,
            UpdatedAt = proveedor.UpdatedAt,
            RowVersion = proveedor.RowVersion
        };
    }
}
