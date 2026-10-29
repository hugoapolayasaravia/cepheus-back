using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.CreateUnidadNegocio
{
    public class CreateUnidadNegocioCommandHandler : IRequestHandler<CreateUnidadNegocioCommand, UnidadNegocioResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateUnidadNegocioCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadNegocioResponse> Handle(CreateUnidadNegocioCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Logistica.Catalogos.UnidadesNegocio.Query().Select(u => u.Code), length: 6, entityLabel: "Unidades de Negocio", cancellationToken);

            var unidad = new UnidadNegocio
            {
                Code = code,
                Name = string.IsNullOrWhiteSpace(request.Name) ? null : request.Name.Trim(),
                ParentCode = string.IsNullOrWhiteSpace(request.ParentCode)
                    ? null
                    : request.ParentCode.Trim().ToUpperInvariant(),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.UnidadesNegocio.AddAsync(unidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(unidad);
        }

        internal static UnidadNegocioResponse Map(UnidadNegocio unidad) => new()
        {
            Code = unidad.Code,
            Name = unidad.Name,
            ParentCode = unidad.ParentCode,
            IsActive = unidad.IsActive,
            CreatedAt = unidad.CreatedAt,
            UpdatedAt = unidad.UpdatedAt,
            CreatedBy = unidad.CreatedBy,
            UpdatedBy = unidad.UpdatedBy,
            RowVersion = unidad.RowVersion
        };
    }
}