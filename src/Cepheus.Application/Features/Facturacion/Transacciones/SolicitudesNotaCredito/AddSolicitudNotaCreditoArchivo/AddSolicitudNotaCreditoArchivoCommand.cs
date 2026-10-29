using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.AddSolicitudNotaCreditoArchivo
{
    public record AddSolicitudNotaCreditoArchivoCommand(
        string Year,
        string Month,
        string Code,
        string FileName,
        string FilePath
    ) : IRequest<SolicitudNotaCreditoArchivoResponse>;
}
