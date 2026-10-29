using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.Common;
using Cepheus.Domain.Mantenimiento.Catalogos;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.CreatePrioridad
{
    public class CreatePrioridadCommandHandler
        : IRequestHandler<CreatePrioridadCommand, PrioridadResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreatePrioridadCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PrioridadResponse> Handle(
            CreatePrioridadCommand request,
            CancellationToken cancellationToken)
        {
            var prioridad = new Prioridad
            {
                Code = request.Code.Trim().ToUpperInvariant(),
                Name = request.Name.Trim(),
                IsActive = true
            };

            await _uow.Mantenimiento.Catalogos.Prioridades.AddAsync(prioridad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(prioridad);
        }

        internal static PrioridadResponse Map(Prioridad prioridad) => new()
        {
            Code = prioridad.Code,
            Name = prioridad.Name,
            IsActive = prioridad.IsActive,
            CreatedAt = prioridad.CreatedAt,
            UpdatedAt = prioridad.UpdatedAt,
            RowVersion = prioridad.RowVersion
        };
    }
}
