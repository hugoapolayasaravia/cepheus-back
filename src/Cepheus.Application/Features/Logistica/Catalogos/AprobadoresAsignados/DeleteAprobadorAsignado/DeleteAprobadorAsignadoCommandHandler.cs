using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.DeleteAprobadorAsignado
{
    public class DeleteAprobadorAsignadoCommandHandler : IRequestHandler<DeleteAprobadorAsignadoCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteAprobadorAsignadoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteAprobadorAsignadoCommand request, CancellationToken cancellationToken)
        {
            var nivel = request.NivelCode.Trim().ToUpperInvariant();
            var trans = request.TipoTransaccionCode.Trim().ToUpperInvariant();
            var une = request.UnidadNegocioCode.Trim().ToUpperInvariant();
            var mon = request.MonedaCode.Trim().ToUpperInvariant();
            var trabajador = request.TrabajadorCode.Trim().ToUpperInvariant();

            var aprobador = await _uow.Logistica.Catalogos.AprobadoresAsignados.Query()
                .FirstOrDefaultAsync(a =>
                    a.NivelCode == nivel && a.TipoTransaccionCode == trans &&
                    a.UnidadNegocioCode == une && a.MonedaCode == mon && a.TrabajadorCode == trabajador,
                    cancellationToken);

            if (aprobador is null)
            {
                throw new KeyNotFoundException($"Aprobador asignado {nivel}/{trans}/{une}/{mon}/{trabajador} no encontrado.");
            }

            _uow.Logistica.Catalogos.AprobadoresAsignados.Remove(aprobador);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
