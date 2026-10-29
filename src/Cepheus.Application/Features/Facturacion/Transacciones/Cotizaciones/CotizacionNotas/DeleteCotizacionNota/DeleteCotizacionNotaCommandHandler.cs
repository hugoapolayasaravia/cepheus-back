using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.DeleteCotizacionNota
{
    public class DeleteCotizacionNotaCommandHandler : IRequestHandler<DeleteCotizacionNotaCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCotizacionNotaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteCotizacionNotaCommand request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var nota = await _uow.Facturacion.Transacciones.CotizacionesNotas.Query()
                .FirstOrDefaultAsync(n => n.NegocioCode == negocio && n.Year == request.Year
                                       && n.Month == request.Month && n.Code == code && n.Sequence == request.Sequence,
                                     cancellationToken);

            if (nota is null)
                throw new KeyNotFoundException($"Nota {request.Sequence} de la cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            _uow.Facturacion.Transacciones.CotizacionesNotas.Remove(nota);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
