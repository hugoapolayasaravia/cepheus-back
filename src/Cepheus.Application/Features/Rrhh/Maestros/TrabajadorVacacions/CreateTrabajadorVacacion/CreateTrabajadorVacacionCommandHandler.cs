using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.CreateTrabajadorVacacion
{
    public class CreateTrabajadorVacacionCommandHandler : IRequestHandler<CreateTrabajadorVacacionCommand, TrabajadorVacacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorVacacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorVacacionResponse> Handle(CreateTrabajadorVacacionCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorVacacion
            {
                TrabajadorCode = request.TrabajadorCode,
                FechaVacaciones = request.FechaVacaciones,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorVacacions.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorVacacionResponse Map(TrabajadorVacacion e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            FechaVacaciones = e.FechaVacaciones,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
