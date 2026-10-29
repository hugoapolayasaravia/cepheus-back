using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.Common;
using Cepheus.Domain.Rrhh.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.CreateTipoAfiliacion
{
    public class CreateTipoAfiliacionCommandHandler : IRequestHandler<CreateTipoAfiliacionCommand, TipoAfiliacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoAfiliacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TipoAfiliacionResponse> Handle(CreateTipoAfiliacionCommand request, CancellationToken cancellationToken)
        {
            var code = await SequentialCodeGenerator.NextAsync(
                _uow.Rrhh.Catalogos.TiposAfiliacion.Query().Select(t => t.Code), length: 3, entityLabel: "TiposAfiliacion", cancellationToken);

            var tipoAfiliacion = new TipoAfiliacion
            {
                Code = code,
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Catalogos.TiposAfiliacion.AddAsync(tipoAfiliacion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(tipoAfiliacion);
        }

        internal static TipoAfiliacionResponse Map(TipoAfiliacion tipoAfiliacion) => new()
        {
            Code = tipoAfiliacion.Code,
            Name = tipoAfiliacion.Name,
            IsActive = tipoAfiliacion.IsActive,
            CreatedAt = tipoAfiliacion.CreatedAt,
            UpdatedAt = tipoAfiliacion.UpdatedAt,
            RowVersion = tipoAfiliacion.RowVersion
        };
    }
}