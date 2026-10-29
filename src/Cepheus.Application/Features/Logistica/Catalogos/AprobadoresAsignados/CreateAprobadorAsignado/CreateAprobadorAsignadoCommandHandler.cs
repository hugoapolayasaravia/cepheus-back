using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.CreateAprobadorAsignado
{
    public class CreateAprobadorAsignadoCommandHandler
        : IRequestHandler<CreateAprobadorAsignadoCommand, AprobadorAsignadoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateAprobadorAsignadoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AprobadorAsignadoResponse> Handle(
            CreateAprobadorAsignadoCommand request, CancellationToken cancellationToken)
        {
            var aprobador = new AprobadorAsignado
            {
                NivelCode = request.NivelCode.Trim().ToUpperInvariant(),
                TipoTransaccionCode = request.TipoTransaccionCode.Trim().ToUpperInvariant(),
                UnidadNegocioCode = request.UnidadNegocioCode.Trim().ToUpperInvariant(),
                MonedaCode = request.MonedaCode.Trim().ToUpperInvariant(),
                TrabajadorCode = request.TrabajadorCode.Trim().ToUpperInvariant(),
                SuplenteTrabajadorCode = Normalize(request.SuplenteTrabajadorCode),
                SuperiorTrabajadorCode = Normalize(request.SuperiorTrabajadorCode),
                IsActive = true
            };

            await _uow.Logistica.Catalogos.AprobadoresAsignados.AddAsync(aprobador, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Map(aprobador);
        }

        private static string? Normalize(string? code)
            => string.IsNullOrWhiteSpace(code) ? null : code.Trim().ToUpperInvariant();

        internal static AprobadorAsignadoResponse Map(AprobadorAsignado a) => new()
        {
            NivelCode = a.NivelCode,
            TipoTransaccionCode = a.TipoTransaccionCode,
            UnidadNegocioCode = a.UnidadNegocioCode,
            MonedaCode = a.MonedaCode,
            TrabajadorCode = a.TrabajadorCode,
            SuplenteTrabajadorCode = a.SuplenteTrabajadorCode,
            SuperiorTrabajadorCode = a.SuperiorTrabajadorCode,
            IsActive = a.IsActive,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt,
            CreatedBy = a.CreatedBy,
            UpdatedBy = a.UpdatedBy,
            RowVersion = a.RowVersion
        };
    }
}
