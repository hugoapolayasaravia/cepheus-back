using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.CreateProveedorContacto
{
    public class CreateProveedorContactoCommandHandler
        : IRequestHandler<CreateProveedorContactoCommand, ProveedorContactoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateProveedorContactoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorContactoResponse> Handle(CreateProveedorContactoCommand request, CancellationToken cancellationToken)
        {
            var proveedorCode = request.ProveedorCode.Trim().ToUpperInvariant();

            // Invariante: un solo contacto principal por proveedor.
            if (request.IsPrimary)
            {
                var otros = await _uow.Logistica.Maestros.ProveedorContactos.Query()
                    .Where(c => c.ProveedorCode == proveedorCode && c.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var otro in otros)
                {
                    otro.IsPrimary = false;
                }
            }

            var contacto = new ProveedorContacto
            {
                ProveedorCode = proveedorCode,
                FirstName = request.FirstName.Trim(),
                LastName = string.IsNullOrWhiteSpace(request.LastName) ? null : request.LastName.Trim(),
                Position = string.IsNullOrWhiteSpace(request.Position) ? null : request.Position.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                MobilePhone = string.IsNullOrWhiteSpace(request.MobilePhone) ? null : request.MobilePhone.Trim(),
                Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
                IsPrimary = request.IsPrimary,
                IsActive = true
            };

            await _uow.Logistica.Maestros.ProveedorContactos.AddAsync(contacto, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(contacto);
        }

        internal static ProveedorContactoResponse Map(ProveedorContacto contacto) => new()
        {
            Id = contacto.Id,
            ProveedorCode = contacto.ProveedorCode,
            FirstName = contacto.FirstName,
            LastName = contacto.LastName,
            Position = contacto.Position,
            Phone = contacto.Phone,
            MobilePhone = contacto.MobilePhone,
            Email = contacto.Email,
            IsPrimary = contacto.IsPrimary,
            IsActive = contacto.IsActive,
            CreatedAt = contacto.CreatedAt,
            UpdatedAt = contacto.UpdatedAt
        };
    }
}
