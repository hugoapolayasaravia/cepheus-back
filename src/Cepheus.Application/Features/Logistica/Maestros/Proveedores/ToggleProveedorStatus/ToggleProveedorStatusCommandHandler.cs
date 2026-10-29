using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.ToggleProveedorStatus
{
    public class ToggleProveedorStatusCommandHandler : IRequestHandler<ToggleProveedorStatusCommand, bool>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public ToggleProveedorStatusCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(ToggleProveedorStatusCommand request, CancellationToken cancellationToken)
        {
            var proveedor = await _uow.Logistica.Maestros.Proveedores.Query()
                .FirstOrDefaultAsync(p => p.Code == request.Code, cancellationToken);

            if (proveedor is null)
            {
                throw new KeyNotFoundException($"Proveedor {request.Code} no encontrado.");
            }

            proveedor.IsActive = !proveedor.IsActive;

            if (!proveedor.IsActive)
            {
                proveedor.DeactivatedAt = DateTime.UtcNow;
                proveedor.DeactivatedBy = _currentUser.FullName;
            }
            else
            {
                proveedor.DeactivatedAt = null;
                proveedor.DeactivatedBy = null;
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return proveedor.IsActive;
        }
    }
}
