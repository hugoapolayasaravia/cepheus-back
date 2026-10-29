using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionNotas.DeleteCotizacionNota
{
    public record DeleteCotizacionNotaCommand(
        string NegocioCode, string Year, string Month, string Code, int Sequence
    ) : IRequest;
}
