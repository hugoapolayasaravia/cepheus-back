using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.DocumentosCompra.DocumentoCompraDetalles.Common
{
    /// <summary>Mismas reglas de edición que el Update de cabecera: solo Pendiente, sin pagos y sin asiento.</summary>
    public static class DocumentoCompraEdicionRules
    {
        public static void EnsureEditable(DocumentoCompra doc, string accion)
        {
            if (doc.Estado != EstadoDocumentoCompra.Pendiente)
            {
                throw new InvalidOperationException(
                    $"El documento está en estado '{doc.Estado}' y ya no admite {accion}.");
            }

            if (doc.PagadoSol > 0)
            {
                throw new InvalidOperationException(
                    $"El documento ya tiene pagos registrados y no admite {accion}.");
            }

            if (!string.IsNullOrWhiteSpace(doc.AsientoProvisionCode))
            {
                throw new InvalidOperationException(
                    $"El documento ya generó el asiento de provisión {doc.AsientoProvisionCode} y no admite {accion}.");
            }
        }
    }
}
