using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.Proveedores.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.GetProveedorByCode
{
    public class GetProveedorByCodeQueryHandler : IRequestHandler<GetProveedorByCodeQuery, ProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorByCodeQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorResponse> Handle(GetProveedorByCodeQuery request, CancellationToken cancellationToken)
        {
            var proveedor = await _uow.Logistica.Maestros.Proveedores.Query()
                .AsNoTracking()
                .Where(p => p.Code == request.Code)
                .Select(p => new ProveedorResponse
                {
                    Code = p.Code,
                    DocumentTypeCode = p.DocumentTypeCode,
                    DocumentNumber = p.DocumentNumber,
                    LegalName = p.LegalName,
                    TradeName = p.TradeName,
                    ProviderType = p.ProviderType,
                    Origin = p.Origin,
                    SunatCondition = p.SunatCondition,
                    SunatStatus = p.SunatStatus,
                    Observations = p.Observations,
                    IsActive = p.IsActive,
                    DeactivatedAt = p.DeactivatedAt,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    RowVersion = p.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (proveedor is null)
            {
                throw new KeyNotFoundException($"Proveedor {request.Code} no encontrado.");
            }

            return proveedor;
        }
    }
}
