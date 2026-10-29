using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.UpdateProveedorContacto
{
    public class UpdateProveedorContactoCommandHandler
        : IRequestHandler<UpdateProveedorContactoCommand, ProveedorContactoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProveedorContactoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorContactoResponse> Handle(UpdateProveedorContactoCommand request, CancellationToken cancellationToken)
        {
            var contacto = await _uow.Logistica.Maestros.ProveedorContactos.GetByIdAsync(request.Id, cancellationToken);

            if (contacto is null)
            {
                throw new KeyNotFoundException($"Contacto {request.Id} no encontrado.");
            }

            if (request.IsPrimary && !contacto.IsPrimary)
            {
                var otros = await _uow.Logistica.Maestros.ProveedorContactos.Query()
                    .Where(c => c.ProveedorCode == contacto.ProveedorCode && c.IsPrimary && c.Id != contacto.Id)
                    .ToListAsync(cancellationToken);

                foreach (var otro in otros)
                {
                    otro.IsPrimary = false;
                }
            }

            contacto.FirstName = request.FirstName.Trim();
            contacto.LastName = string.IsNullOrWhiteSpace(request.LastName) ? null : request.LastName.Trim();
            contacto.Position = string.IsNullOrWhiteSpace(request.Position) ? null : request.Position.Trim();
            contacto.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
            contacto.MobilePhone = string.IsNullOrWhiteSpace(request.MobilePhone) ? null : request.MobilePhone.Trim();
            contacto.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            contacto.IsPrimary = request.IsPrimary;

            await _uow.SaveChangesAsync(cancellationToken);

            return CreateProveedorContacto.CreateProveedorContactoCommandHandler.Map(contacto);
        }
    }
}
