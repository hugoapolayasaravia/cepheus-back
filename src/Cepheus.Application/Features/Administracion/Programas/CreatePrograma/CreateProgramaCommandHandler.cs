using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Programas.Common;
using Cepheus.Domain.Administracion;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Programas.CreatePrograma
{
    public class CreateProgramaCommandHandler : IRequestHandler<CreateProgramaCommand, ProgramaResponse>
    {
        /// <summary>
        /// CRUD básico que se auto-genera para TODO Programa nuevo. Permisos
        /// adicionales específicos (ej. "ANNUL" para Notas de Ingreso) se agregan
        /// después, a mano, vía el CRUD manual de Permission.
        /// </summary>
        private static readonly (string Code, string Name)[] StandardPermissions =
        {
        ("VIEW", "Consultar"),
        ("CREATE", "Crear"),
        ("UPDATE", "Modificar"),
        ("DELETE", "Eliminar")
    };

        private readonly IUnitOfWork _uow;

        public CreateProgramaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ProgramaResponse> Handle(CreateProgramaCommand request, CancellationToken cancellationToken)
        {
            var programa = new Programa
            {
                SubmoduloId = request.SubmoduloId,
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim(),
                Tooltip = string.IsNullOrWhiteSpace(request.Tooltip) ? null : request.Tooltip.Trim(),
                Route = string.IsNullOrWhiteSpace(request.Route) ? null : request.Route.Trim(),
                DisplayOrder = request.DisplayOrder,
                IsActive = true
            };

            await _uow.Administracion.Programas.AddAsync(programa, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken); // necesita Id antes de crear los Permissions

            var permissions = new List<Permission>();

            foreach (var (code, name) in StandardPermissions)
            {
                var permission = new Permission
                {
                    ProgramaId = programa.Id,
                    Code = code,
                    Name = name,
                    IsActive = true
                };

                permissions.Add(permission);
                await _uow.Administracion.Permissions.AddAsync(permission, cancellationToken);
            }

            await _uow.SaveChangesAsync(cancellationToken);

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
                    .Select(p => new PermissionSummary { Id = p.Id, Code = p.Code, Name = p.Name })
                    .ToList()
            };
        }
    }


}
