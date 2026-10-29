using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.Common;
using Cepheus.Domain.Rrhh.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.CreateTrabajadorContrato
{
    public class CreateTrabajadorContratoCommandHandler : IRequestHandler<CreateTrabajadorContratoCommand, TrabajadorContratoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateTrabajadorContratoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<TrabajadorContratoResponse> Handle(CreateTrabajadorContratoCommand request, CancellationToken cancellationToken)
        {
            var entidad = new TrabajadorContrato
            {
                TrabajadorCode = request.TrabajadorCode,
                TipoContratoCode = request.TipoContratoCode,
                TipoExtensionCode = request.TipoExtensionCode,
                FechaInicio = request.FechaInicio,
                FechaFin = request.FechaFin,
                FechaTermino = request.FechaTermino,
                Renovado = request.Renovado,
                TipoDuracion = string.IsNullOrWhiteSpace(request.TipoDuracion) ? null : request.TipoDuracion!.Trim(),
                CantidadDuracion = request.CantidadDuracion,
                Activo = request.Activo,
                IsActive = true
            };

            await _uow.Rrhh.Maestros.TrabajadorContratos.AddAsync(entidad, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(entidad);
        }

        internal static TrabajadorContratoResponse Map(TrabajadorContrato e) => new()
        {
            Id = e.Id,
            TrabajadorCode = e.TrabajadorCode,
            TipoContratoCode = e.TipoContratoCode,
            TipoExtensionCode = e.TipoExtensionCode,
            FechaInicio = e.FechaInicio,
            FechaFin = e.FechaFin,
            FechaTermino = e.FechaTermino,
            Renovado = e.Renovado,
            TipoDuracion = e.TipoDuracion,
            CantidadDuracion = e.CantidadDuracion,
            Activo = e.Activo,
            IsActive = e.IsActive,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            RowVersion = e.RowVersion
        };
    }
}
