using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.Common;
using Cepheus.Domain.Contabilidad.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.CreateNoAfectoRetencion
{
    public class CreateNoAfectoRetencionCommandHandler
        : IRequestHandler<CreateNoAfectoRetencionCommand, NoAfectoRetencionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateNoAfectoRetencionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NoAfectoRetencionResponse> Handle(
            CreateNoAfectoRetencionCommand request,
            CancellationToken cancellationToken)
        {
            var entity = new NoAfectoRetencion
            {
                Ruc = request.Ruc.Trim(),
                Type = request.Type.Trim().ToUpper(),
                Name = request.Name.Trim(),
                Resolution = request.Resolution.Trim(),
                IsActive = true
            };

            await _uow.Contabilidad.Maestros.NoAfectosRetencion.AddAsync(entity, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entity);
        }

        internal static NoAfectoRetencionResponse Map(NoAfectoRetencion e) => new()
        {
            Ruc = e.Ruc,
            Type = e.Type,
            Name = e.Name,
            Resolution = e.Resolution,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            CreatedBy = e.CreatedBy,
            UpdatedBy = e.UpdatedBy,
            RowVersion = e.RowVersion
        };
    }
}
