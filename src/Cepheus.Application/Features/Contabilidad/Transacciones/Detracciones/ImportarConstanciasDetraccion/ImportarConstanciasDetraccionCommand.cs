using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.ImportarConstanciasDetraccion
{
    /// <summary>
    /// Importa el archivo de constancias de depósito del banco y registra operación, constancia y fecha
    /// en los documentos de compra exportados del lote.
    /// </summary>
    public record ImportarConstanciasDetraccionCommand(
        string Year,
        string Period,
        string LotNumber,
        string Content
    ) : IRequest<ImportarConstanciasDetraccionResponse>;

    public class ImportarConstanciasDetraccionResponse
    {
        /// <summary>Constancias leídas del archivo.</summary>
        public int Records { get; set; }

        /// <summary>Documentos de compra actualizados.</summary>
        public int UpdatedDocuments { get; set; }

        /// <summary>Constancias que no se pudieron aplicar, con el motivo.</summary>
        public List<string> Errors { get; set; } = new();
    }
}
