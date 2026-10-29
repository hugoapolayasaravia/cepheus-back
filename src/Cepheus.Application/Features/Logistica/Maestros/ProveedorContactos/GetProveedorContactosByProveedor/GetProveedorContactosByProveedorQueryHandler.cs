using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.GetProveedorContactosByProveedor
{
    public class GetProveedorContactosByProveedorQueryHandler
        : IRequestHandler<GetProveedorContactosByProveedorQuery, List<ProveedorContactoResponse>>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorContactosByProveedorQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ProveedorContactoResponse>> Handle(
            GetProveedorContactosByProveedorQuery request, CancellationToken cancellationToken)
        {
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            return await _uow.Logistica.Maestros.ProveedorContactos.Query()
                .AsNoTracking()
                .Where(c => c.ProveedorCode == proveedorCode)
                .OrderByDescending(c => c.IsPrimary)
                .ThenBy(c => c.Id)
                .Select(c => new ProveedorContactoResponse
                {
                    Id = c.Id,
                    ProveedorCode = c.ProveedorCode,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Position = c.Position,
                    Phone = c.Phone,
                    MobilePhone = c.MobilePhone,
                    Email = c.Email,
                    IsPrimary = c.IsPrimary,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedBy = c.UpdatedBy
                })
                .ToListAsync(cancellationToken);
        }
    }
}
