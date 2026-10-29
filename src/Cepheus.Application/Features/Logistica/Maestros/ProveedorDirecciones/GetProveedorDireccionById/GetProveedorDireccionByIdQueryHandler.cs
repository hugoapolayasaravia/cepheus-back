using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorDirecciones.GetProveedorDireccionById
{
    public class GetProveedorDireccionByIdQueryHandler : IRequestHandler<GetProveedorDireccionByIdQuery, ProveedorDireccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetProveedorDireccionByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProveedorDireccionResponse> Handle(GetProveedorDireccionByIdQuery request, CancellationToken cancellationToken)
        {
            var direccion = await _uow.Logistica.Maestros.ProveedorDirecciones.Query()
                .AsNoTracking()
                .Where(d => d.Id == request.Id)
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
                    UpdatedAt = d.UpdatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (direccion is null)
            {
                throw new KeyNotFoundException($"Dirección {request.Id} no encontrada.");
            }

            return direccion;
        }
    }
}
