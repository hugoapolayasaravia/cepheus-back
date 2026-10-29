using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.GenerarArchivoDetraccion
{
    /// <summary>
    /// Genera el archivo de depósito masivo (TXT) del lote. Si el lote está Pendiente pasa a Emitido y
    /// se fija la fecha de depósito (legacy: ue_set_btn_genera_detraccion). Se puede volver a descargar.
    /// </summary>
    public record GenerarArchivoDetraccionCommand(string Year, string Period, string LotNumber)
        : IRequest<ExportFileResponse>;
}
