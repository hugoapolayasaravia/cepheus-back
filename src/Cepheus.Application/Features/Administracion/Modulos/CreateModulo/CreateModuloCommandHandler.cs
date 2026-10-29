using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Modulos.Common;
using Cepheus.Domain.Administracion;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Modulos.CreateModulo
{
    public class CreateModuloCommandHandler : IRequestHandler<CreateModuloCommand, ModuloResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateModuloCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ModuloResponse> Handle(CreateModuloCommand request, CancellationToken cancellationToken)
        {
            var modulo = new Modulo
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                Icon = string.IsNullOrWhiteSpace(request.Icon) ? null : request.Icon.Trim(),
                Tooltip = request.Tooltip?.Trim() ?? string.Empty,
                DisplayOrder = request.DisplayOrder,
                IsActive = true
            };

            await _uow.Administracion.Modulos.AddAsync(modulo, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

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
