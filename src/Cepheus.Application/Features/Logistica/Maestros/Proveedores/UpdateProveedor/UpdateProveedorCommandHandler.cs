using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Proveedores.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.UpdateProveedor
{
    public class UpdateProveedorCommandHandler : IRequestHandler<UpdateProveedorCommand, ProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorResponse> Handle(UpdateProveedorCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Logistica.Maestros.Proveedores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Proveedor {request.Code} no encontrado.");
            }

            var proveedor = new Proveedor
            {
                Code = request.Code,
                DocumentTypeCode = request.DocumentTypeCode.Trim().ToUpperInvariant(),
                DocumentNumber = request.DocumentNumber.Trim(),
                LegalName = request.LegalName.Trim(),
                TradeName = string.IsNullOrWhiteSpace(request.TradeName) ? null : request.TradeName.Trim(),
                ProviderType = request.ProviderType,
                Origin = request.Origin,
                SunatCondition = request.SunatCondition,
                SunatStatus = request.SunatStatus,
                Observations = string.IsNullOrWhiteSpace(request.Observations) ? null : request.Observations.Trim(),

                IsActive = current.IsActive,
                DeactivatedAt = current.DeactivatedAt,
                DeactivatedBy = current.DeactivatedBy,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Maestros.Proveedores.Update(proveedor);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El proveedor fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ProveedorResponse
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
}
