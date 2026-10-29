using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Programas.Common;
using Cepheus.Domain.Administracion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Programas.UpdatePrograma
{
    public class UpdateProgramaCommandHandler : IRequestHandler<UpdateProgramaCommand, ProgramaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProgramaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProgramaResponse> Handle(UpdateProgramaCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Administracion.Programas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Programa {request.Id} no encontrado.");
            }

            var programa = new Programa
            {
                Id = request.Id,
                SubmoduloId = current.SubmoduloId, // no editable acá
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim(),
                Tooltip = string.IsNullOrWhiteSpace(request.Tooltip) ? null : request.Tooltip.Trim(),
                Route = string.IsNullOrWhiteSpace(request.Route) ? null : request.Route.Trim(),
                DisplayOrder = request.DisplayOrder,

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Administracion.Programas.Update(programa);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El programa fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            var permissions = await _uow.Administracion.Permissions.Query()
                .AsNoTracking()
                .Where(p => p.ProgramaId == programa.Id)
                .Select(p => new PermissionSummary { Id = p.Id, Code = p.Code, Name = p.Name })
                .ToListAsync(cancellationToken);

            return new ProgramaResponse
            {
                Id = programa.Id,
                SubmoduloId = programa.SubmoduloId,
                Code = programa.Code,
                Name = programa.Name,
                Icon = programa.Icon,
                Tooltip = programa.Tooltip,
                Route = programa.Route,
                DisplayOrder = programa.DisplayOrder,
                IsActive = programa.IsActive,
                CreatedAt = programa.CreatedAt,
                UpdatedAt = programa.UpdatedAt,
                CreatedBy = programa.CreatedBy,
                UpdatedBy = programa.UpdatedBy,
                RowVersion = programa.RowVersion,
                Permissions = permissions
            };
        }
    }


}
