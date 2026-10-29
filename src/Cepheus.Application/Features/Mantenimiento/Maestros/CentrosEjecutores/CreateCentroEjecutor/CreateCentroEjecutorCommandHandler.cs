using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.Common;
using Cepheus.Domain.Mantenimiento.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.CreateCentroEjecutor
{
    public class CreateCentroEjecutorCommandHandler
        : IRequestHandler<CreateCentroEjecutorCommand, CentroEjecutorResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCentroEjecutorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CentroEjecutorResponse> Handle(
            CreateCentroEjecutorCommand request,
            CancellationToken cancellationToken)
        {
            var centroEjecutor = new CentroEjecutor
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Mantenimiento.Maestros.CentrosEjecutores.AddAsync(centroEjecutor, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(centroEjecutor);
        }

        internal static CentroEjecutorResponse Map(CentroEjecutor centroEjecutor) => new()
        {
            Code = centroEjecutor.Code,
            Name = centroEjecutor.Name,
            IsActive = centroEjecutor.IsActive,
            CreatedAt = centroEjecutor.CreatedAt,
            UpdatedAt = centroEjecutor.UpdatedAt,
            RowVersion = centroEjecutor.RowVersion
        };
    }
}
