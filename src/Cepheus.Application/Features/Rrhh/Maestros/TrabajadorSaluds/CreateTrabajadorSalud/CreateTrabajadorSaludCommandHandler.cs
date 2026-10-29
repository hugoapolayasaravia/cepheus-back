using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.CreateTrabajadorSalud
{
    public class CreateTrabajadorSaludCommandHandler : IRequestHandler<CreateTrabajadorSaludCommand, TrabajadorSaludResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorSaludCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorSaludResponse> Handle(CreateTrabajadorSaludCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorSalud
            {
                TrabajadorCode = request.TrabajadorCode,
                TipoSangreCode = request.TipoSangreCode,
                AlergiaCode = request.AlergiaCode,
                Otros = string.IsNullOrWhiteSpace(request.Otros) ? null : request.Otros!.Trim(),
                FechaEvaluacionMedica = request.FechaEvaluacionMedica,
                Observaciones = string.IsNullOrWhiteSpace(request.Observaciones) ? null : request.Observaciones!.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorSaluds.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorSaludResponse Map(TrabajadorSalud e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            TipoSangreCode = e.TipoSangreCode,
            AlergiaCode = e.AlergiaCode,
            Otros = e.Otros,
            FechaEvaluacionMedica = e.FechaEvaluacionMedica,
            Observaciones = e.Observaciones,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
