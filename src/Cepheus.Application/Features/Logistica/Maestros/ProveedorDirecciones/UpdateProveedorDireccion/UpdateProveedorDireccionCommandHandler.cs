using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.UpdateProveedorDireccion
{
    public class UpdateProveedorDireccionCommandHandler
        : IRequestHandler<UpdateProveedorDireccionCommand, ProveedorDireccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProveedorDireccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorDireccionResponse> Handle(UpdateProveedorDireccionCommand request, CancellationToken cancellationToken)
        {
            var direccion = await _uow.Logistica.Maestros.ProveedorDirecciones.GetByIdAsync(request.Id, cancellationToken);

            if (direccion is null)
            {
                throw new KeyNotFoundException($"Dirección {request.Id} no encontrada.");
            }

            // Invariante: una sola dirección principal por proveedor.
            if (request.IsPrimary && !direccion.IsPrimary)
            {
                var otras = await _uow.Logistica.Maestros.ProveedorDirecciones.Query()
                    .Where(d => d.ProveedorCode == direccion.ProveedorCode && d.IsPrimary && d.Id != direccion.Id)
                    .ToListAsync(cancellationToken);

                foreach (var otra in otras)
                {
                    otra.IsPrimary = false;
                }
            }

            direccion.AddressType = request.AddressType;
            direccion.Address = request.Address.Trim();
            direccion.UbigeoCode = string.IsNullOrWhiteSpace(request.UbigeoCode) ? null : request.UbigeoCode.Trim().ToUpperInvariant();
            direccion.Reference = string.IsNullOrWhiteSpace(request.Reference) ? null : request.Reference.Trim();
            direccion.IsPrimary = request.IsPrimary;

            await _uow.SaveChangesAsync(cancellationToken);

            return CreateProveedorDireccion.CreateProveedorDireccionCommandHandler.Map(direccion);
        }
    }
}
