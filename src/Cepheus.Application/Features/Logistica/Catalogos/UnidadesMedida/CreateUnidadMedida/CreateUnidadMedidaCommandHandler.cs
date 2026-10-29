using Cepheus.Application.Comun.Helpers;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.Common;
using Cepheus.Domain.Logistica.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesMedida.CreateUnidadMedida
{
    public class CreateUnidadMedidaCommandHandler
    : IRequestHandler<CreateUnidadMedidaCommand, UnidadMedidaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateUnidadMedidaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<UnidadMedidaResponse> Handle(
            CreateUnidadMedidaCommand request,
            CancellationToken cancellationToken)
        {
            var unidad = new UnidadMedida
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IndDecimal = request.IndDecimal,
                IsActive = true
            };

            await _uow.Logistica.Catalogos.UnidadesMedida.AddAsync(unidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(unidad);
        }

        internal static UnidadMedidaResponse Map(UnidadMedida unidad) => new()
        {
            Code = unidad.Code,
            Name = unidad.Name,
            IndDecimal = unidad.IndDecimal,
            IsActive = unidad.IsActive,
            CreatedAt = unidad.CreatedAt,
            UpdatedAt = unidad.UpdatedAt,
            CreatedBy = unidad.CreatedBy,
            UpdatedBy = unidad.UpdatedBy,
            RowVersion = unidad.RowVersion
        };
    }
}