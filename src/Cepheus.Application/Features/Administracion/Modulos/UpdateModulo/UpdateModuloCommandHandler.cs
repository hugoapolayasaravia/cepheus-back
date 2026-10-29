using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Modulos.Common;
using Cepheus.Domain.Administracion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Modulos.UpdateModulo
{
    public class UpdateModuloCommandHandler : IRequestHandler<UpdateModuloCommand, ModuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateModuloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ModuloResponse> Handle(UpdateModuloCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Administracion.Modulos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Módulo {request.Id} no encontrado.");
            }

            var modulo = new Modulo
            {
                Id = request.Id,
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim(),
                Tooltip = request.Tooltip?.Trim() ?? string.Empty,
                DisplayOrder = request.DisplayOrder,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Administracion.Modulos.Update(modulo);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El módulo fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ModuloResponse
            {
                Id = modulo.Id,
                Code = modulo.Code,
                Name = modulo.Name,
                Icon = modulo.Icon,
                Tooltip = modulo.Tooltip,
                DisplayOrder = modulo.DisplayOrder,
                IsActive = modulo.IsActive,
                CreatedAt = modulo.CreatedAt,
                UpdatedAt = modulo.UpdatedAt,
                CreatedBy = modulo.CreatedBy,
                UpdatedBy = modulo.UpdatedBy,
                RowVersion = modulo.RowVersion
            };
        }
    }



}
