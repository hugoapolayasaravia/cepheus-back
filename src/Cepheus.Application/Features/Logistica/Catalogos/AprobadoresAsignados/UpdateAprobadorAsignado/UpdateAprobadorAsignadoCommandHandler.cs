using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.Common;
using Cepheus.Domain.Logistica.Maestros;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.UpdateAprobadorAsignado
{
    public class UpdateAprobadorAsignadoCommandHandler
        : IRequestHandler<UpdateAprobadorAsignadoCommand, AprobadorAsignadoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateAprobadorAsignadoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AprobadorAsignadoResponse> Handle(
            UpdateAprobadorAsignadoCommand request, CancellationToken cancellationToken)
        {
            var nivel = request.NivelCode.Trim().ToUpperInvariant();
            var trans = request.TipoTransaccionCode.Trim().ToUpperInvariant();
            var une = request.UnidadNegocioCode.Trim().ToUpperInvariant();
            var mon = request.MonedaCode.Trim().ToUpperInvariant();
            var trabajador = request.TrabajadorCode.Trim().ToUpperInvariant();

            var current = await _uow.Logistica.Catalogos.AprobadoresAsignados.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(a =>
                    a.NivelCode == nivel && a.TipoTransaccionCode == trans &&
                    a.UnidadNegocioCode == une && a.MonedaCode == mon && a.TrabajadorCode == trabajador,
                    cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Aprobador asignado {nivel}/{trans}/{une}/{mon}/{trabajador} no encontrado.");
            }

            var aprobador = new AprobadorAsignado
            {
                NivelCode = nivel,
                TipoTransaccionCode = trans,
                UnidadNegocioCode = une,
                MonedaCode = mon,
                TrabajadorCode = trabajador,

                SuplenteTrabajadorCode = string.IsNullOrWhiteSpace(request.SuplenteTrabajadorCode)
                    ? null : request.SuplenteTrabajadorCode.Trim().ToUpperInvariant(),
                SuperiorTrabajadorCode = string.IsNullOrWhiteSpace(request.SuperiorTrabajadorCode)
                    ? null : request.SuperiorTrabajadorCode.Trim().ToUpperInvariant(),

                IsActive = current.IsActive,
                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Logistica.Catalogos.AprobadoresAsignados.Update(aprobador);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El aprobador asignado fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreateAprobadorAsignado.CreateAprobadorAsignadoCommandHandler.Map(aprobador);
        }
    }
}
