using Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.UpdateSolicitudNotaCredito
{
    public record UpdateSolicitudNotaCreditoCommand(
        string Year,
        string Month,
        string Code,
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
        List<SolicitudNotaCreditoGuiaInput>? Guides,
        byte[] RowVersion
    ) : ISolicitudNotaCreditoInput, IRequest<SolicitudNotaCreditoResponse>;
}
