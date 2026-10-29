using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Modulos.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Modulos.GetModuloById
{
    public class GetModuloByIdQueryHandler : IRequestHandler<GetModuloByIdQuery, ModuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetModuloByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ModuloResponse> Handle(GetModuloByIdQuery request, CancellationToken cancellationToken)
        {
            var modulo = await _uow.Administracion.Modulos.Query()
                .AsNoTracking()
                .Where(m => m.Id == request.Id)
                .Select(m => new ModuloResponse
                {
                    Id = m.Id,
                    Code = m.Code,
                    Name = m.Name,
                    Icon = m.Icon,
                    Tooltip = m.Tooltip,
                    DisplayOrder = m.DisplayOrder,
                    IsActive = m.IsActive,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    CreatedBy = m.CreatedBy,
                    UpdatedBy = m.UpdatedBy,
                    RowVersion = m.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (modulo is null)
            {
                throw new KeyNotFoundException($"Módulo {request.Id} no encontrado.");
            }

            return modulo;
        }
    }


}
