using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.CreateOportunidad
{
    public class CreateOportunidadCommandHandler
        : IRequestHandler<CreateOportunidadCommand, OportunidadResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateOportunidadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OportunidadResponse> Handle(
            CreateOportunidadCommand request,
            CancellationToken cancellationToken)
        {
            var oportunidad = new Oportunidad
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Mantenimiento.Catalogos.Oportunidades.AddAsync(oportunidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(oportunidad);
        }

        internal static OportunidadResponse Map(Oportunidad oportunidad) => new()
        {
            Code = oportunidad.Code,
            Name = oportunidad.Name,
            IsActive = oportunidad.IsActive,
            CreatedAt = oportunidad.CreatedAt,
            UpdatedAt = oportunidad.UpdatedAt,
            RowVersion = oportunidad.RowVersion
        };
    }
}
