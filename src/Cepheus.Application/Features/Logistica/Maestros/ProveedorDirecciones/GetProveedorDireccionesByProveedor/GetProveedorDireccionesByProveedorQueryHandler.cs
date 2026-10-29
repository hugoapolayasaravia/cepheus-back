using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.GetProveedorDireccionesByProveedor
{
    public class GetProveedorDireccionesByProveedorQueryHandler
        : IRequestHandler<GetProveedorDireccionesByProveedorQuery, List<ProveedorDireccionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorDireccionesByProveedorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ProveedorDireccionResponse>> Handle(
            GetProveedorDireccionesByProveedorQuery request, CancellationToken cancellationToken)
        {
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            return await _uow.Logistica.Maestros.ProveedorDirecciones.Query()
                .AsNoTracking()
                .Where(d => d.ProveedorCode == proveedorCode)
                .OrderByDescending(d => d.IsPrimary)
                .ThenBy(d => d.Id)
                .Select(d => new ProveedorDireccionResponse
                {
                    Id = d.Id,
                    ProveedorCode = d.ProveedorCode,
                    AddressType = d.AddressType,
                    Address = d.Address,
                    UbigeoCode = d.UbigeoCode,
                    Reference = d.Reference,
                    IsPrimary = d.IsPrimary,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    CreatedBy = d.CreatedBy,
                    UpdatedBy = d.UpdatedBy
                })
                .ToListAsync(cancellationToken);
        }
    }
}
