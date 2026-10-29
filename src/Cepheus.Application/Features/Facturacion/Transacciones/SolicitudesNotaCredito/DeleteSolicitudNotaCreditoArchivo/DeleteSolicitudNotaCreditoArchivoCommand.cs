using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.DeleteSolicitudNotaCreditoArchivo
{
    public record DeleteSolicitudNotaCreditoArchivoCommand(string Year, string Month, string Code, string FileName) : IRequest<bool>;
}
