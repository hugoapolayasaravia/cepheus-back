using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.UpdateTrabajadorContacto
{
    public class UpdateTrabajadorContactoCommandHandler
        : IRequestHandler<UpdateTrabajadorContactoCommand, TrabajadorContactoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateTrabajadorContactoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorContactoResponse> Handle(UpdateTrabajadorContactoCommand request, CancellationToken cancellationToken)
        {
            var contacto = await _uow.Rrhh.Maestros.TrabajadorContactos.GetByIdAsync(request.Id, cancellationToken);

            if (contacto is null)
            {
                throw new KeyNotFoundException($"Contacto {request.Id} no encontrado.");
            }

            if (request.IsPrimary && !contacto.IsPrimary)
            {
                var otros = await _uow.Rrhh.Maestros.TrabajadorContactos.Query()
                    .Where(c => c.TrabajadorCode == contacto.TrabajadorCode && c.IsPrimary && c.Id != contacto.Id)
                    .ToListAsync(cancellationToken);

                foreach (var otro in otros)
                {
                    otro.IsPrimary = false;
                }
            }

            contacto.Name = request.Name.Trim();
            contacto.Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();
            contacto.ParentescoCode = string.IsNullOrWhiteSpace(request.ParentescoCode) ? null : request.ParentescoCode.Trim().ToUpperInvariant();
            contacto.IsPrimary = request.IsPrimary;

            await _uow.SaveChangesAsync(cancellationToken);

            return CreateTrabajadorContacto.CreateTrabajadorContactoCommandHandler.Map(contacto);
        }
    }
}