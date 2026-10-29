using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.CreateProveedorDireccion
{
    public class CreateProveedorDireccionCommandHandler
        : IRequestHandler<CreateProveedorDireccionCommand, ProveedorDireccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateProveedorDireccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorDireccionResponse> Handle(CreateProveedorDireccionCommand request, CancellationToken cancellationToken)
        {
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            // Invariante: una sola dirección principal por proveedor.
            if (request.IsPrimary)
            {
                var otras = await _uow.Logistica.Maestros.ProveedorDirecciones.Query()
                    .Where(d => d.ProveedorCode == proveedorCode && d.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var otra in otras)
                {
                    otra.IsPrimary = false;
                }
            }

            var direccion = new ProveedorDireccion
            {
                ProveedorCode = proveedorCode,
                AddressType = request.AddressType,
                Address = request.Address.Trim(),
                UbigeoCode = string.IsNullOrWhiteSpace(request.UbigeoCode) ? null : request.UbigeoCode.Trim().ToUpperInvariant(),
                Reference = string.IsNullOrWhiteSpace(request.Reference) ? null : request.Reference.Trim(),
                IsPrimary = request.IsPrimary,
                IsActive = true
            };

            await _uow.Logistica.Maestros.ProveedorDirecciones.AddAsync(direccion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(direccion);
        }

        internal static ProveedorDireccionResponse Map(ProveedorDireccion direccion) => new()
        {
            Id = direccion.Id,
            ProveedorCode = direccion.ProveedorCode,
            AddressType = direccion.AddressType,
            Address = direccion.Address,
            UbigeoCode = direccion.UbigeoCode,
            Reference = direccion.Reference,
            IsPrimary = direccion.IsPrimary,
            IsActive = direccion.IsActive,
            CreatedAt = direccion.CreatedAt,
            UpdatedAt = direccion.UpdatedAt
        };
    }
}
