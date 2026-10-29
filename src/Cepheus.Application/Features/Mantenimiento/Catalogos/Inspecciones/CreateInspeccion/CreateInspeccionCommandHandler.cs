using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Inspecciones.CreateInspeccion
{
    public class CreateInspeccionCommandHandler
        : IRequestHandler<CreateInspeccionCommand, InspeccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateInspeccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<InspeccionResponse> Handle(
            CreateInspeccionCommand request,
            CancellationToken cancellationToken)
        {
            var inspeccion = new Inspeccion
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Mantenimiento.Catalogos.Inspecciones.AddAsync(inspeccion, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(inspeccion);
        }

        internal static InspeccionResponse Map(Inspeccion inspeccion) => new()
        {
            Code = inspeccion.Code,
            Name = inspeccion.Name,
            IsActive = inspeccion.IsActive,
            CreatedAt = inspeccion.CreatedAt,
            UpdatedAt = inspeccion.UpdatedAt,
            RowVersion = inspeccion.RowVersion
        };
    }
}
