using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.CreateTrabajadorPension
{
    public class CreateTrabajadorPensionCommandHandler : IRequestHandler<CreateTrabajadorPensionCommand, TrabajadorPensionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorPensionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorPensionResponse> Handle(CreateTrabajadorPensionCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorPension
            {
                TrabajadorCode = request.TrabajadorCode,
                TipoAfiliacionCode = request.TipoAfiliacionCode,
                AfpCode = request.AfpCode,
                FechaAfiliacion = request.FechaAfiliacion,
                NumeroAfp = string.IsNullOrWhiteSpace(request.NumeroAfp) ? null : request.NumeroAfp!.Trim(),
                RegimenPensionarioCode = request.RegimenPensionarioCode,
                TipoPensionCode = request.TipoPensionCode,
                NumeroCarnetSsp = string.IsNullOrWhiteSpace(request.NumeroCarnetSsp) ? null : request.NumeroCarnetSsp!.Trim(),
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorPensions.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorPensionResponse Map(TrabajadorPension e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            TipoAfiliacionCode = e.TipoAfiliacionCode,
            AfpCode = e.AfpCode,
            FechaAfiliacion = e.FechaAfiliacion,
            NumeroAfp = e.NumeroAfp,
            RegimenPensionarioCode = e.RegimenPensionarioCode,
            TipoPensionCode = e.TipoPensionCode,
            NumeroCarnetSsp = e.NumeroCarnetSsp,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
