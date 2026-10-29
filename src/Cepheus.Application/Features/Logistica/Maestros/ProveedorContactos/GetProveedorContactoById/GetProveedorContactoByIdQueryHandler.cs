using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.GetProveedorContactoById
{
    public class GetProveedorContactoByIdQueryHandler : IRequestHandler<GetProveedorContactoByIdQuery, ProveedorContactoResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorContactoByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorContactoResponse> Handle(GetProveedorContactoByIdQuery request, CancellationToken cancellationToken)
        {
            var contacto = await _uow.Logistica.Maestros.ProveedorContactos.Query()
                .AsNoTracking()
                .Where(c => c.Id == request.Id)
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
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (contacto is null)
            {
                throw new KeyNotFoundException($"Contacto {request.Id} no encontrado.");
            }

            return contacto;
        }
    }
}
