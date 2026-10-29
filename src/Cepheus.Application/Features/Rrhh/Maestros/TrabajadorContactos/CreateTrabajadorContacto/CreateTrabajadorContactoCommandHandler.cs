using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.CreateTrabajadorContacto
{
    public class CreateTrabajadorContactoCommandHandler
        : IRequestHandler<CreateTrabajadorContactoCommand, TrabajadorContactoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorContactoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorContactoResponse> Handle(CreateTrabajadorContactoCommand request, CancellationToken cancellationToken)
        {
            var trabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant();

            if (request.IsPrimary)
            {
                var otros = await _uow.Rrhh.Maestros.TrabajadorContactos.Query()
                    .Where(c => c.TrabajadorCode == trabajadorCode && c.IsPrimary)
                    .ToListAsync(cancellationToken);

                foreach (var otro in otros)
                {
                    otro.IsPrimary = false;
                }
            }

            var contacto = new TrabajadorContacto
            {
                TrabajadorCode = trabajadorCode,
                Name = request.Name.Trim(),
                Phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
                ParentescoCode = string.IsNullOrWhiteSpace(request.ParentescoCode) ? null : request.ParentescoCode.Trim().ToUpperInvariant(),
                IsPrimary = request.IsPrimary
            };

            await _uow.Rrhh.Maestros.TrabajadorContactos.AddAsync(contacto, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(contacto);
        }

        internal static TrabajadorContactoResponse Map(TrabajadorContacto contacto) => new()
        {
            Id = contacto.Id,
            TrabajadorCode = contacto.TrabajadorCode,
            Name = contacto.Name,
            Phone = contacto.Phone,
            ParentescoCode = contacto.ParentescoCode,
            IsPrimary = contacto.IsPrimary,
            CreatedAt = contacto.CreatedAt,
            UpdatedAt = contacto.UpdatedAt
        };
    }
}