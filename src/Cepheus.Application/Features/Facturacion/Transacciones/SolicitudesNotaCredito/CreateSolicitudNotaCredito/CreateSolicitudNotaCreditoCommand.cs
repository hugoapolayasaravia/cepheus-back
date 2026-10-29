using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.CreateSolicitudNotaCredito
{
    public record CreateSolicitudNotaCreditoCommand(
        string NegocioCode,
        DateTime IssueDate,
        string ClienteCode,
        string DocumentTypeCode,
        string DocumentNumber,
        string MotivoCode,
        string MonedaCode,
        string Observation,
        bool? IsProcedent,
        bool? RequesterAccepts,
        List<SolicitudNotaCreditoDetalleInput> Details,
        List<SolicitudNotaCreditoGuiaInput>? Guides
    ) : ISolicitudNotaCreditoInput, IRequest<SolicitudNotaCreditoResponse>;
}
