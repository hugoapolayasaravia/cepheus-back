using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common
{
    /// <summary>
    /// Reglas de la cobranza dudosa (legacy w_con_cobranzadudosa).
    /// Tipos que admite: factura (01), boleta (03), nota de débito (08), letra (10) y cheque (11).
    /// La nota de crédito (07) no se admite: el legacy la dejaba registrar pero su canje fallaba siempre
    /// (la línea 12 de su provisión es al Haber y el canje busca una línea 12 al Debe).
    /// </summary>
    public static class CobranzaDudosaRules
    {
        public static readonly string[] TiposPermitidos =
        {
            CobranzaTipoDocumento.Factura, CobranzaTipoDocumento.Boleta, CobranzaTipoDocumento.NotaDebito,
            CobranzaTipoDocumento.Letra, CobranzaTipoDocumento.Cheque
        };

        /// <summary>La planta forma parte de la clave del documento (el número se repite entre plantas).</summary>
        public static bool UsaPlanta(string typeCode) => typeCode is CobranzaTipoDocumento.NotaDebito or CobranzaTipoDocumento.Letra;

        /// <summary>Documento sin obra propia: letras y cheques (el legacy deja la obra en blanco).</summary>
        public static bool SinObra(string typeCode) => typeCode is CobranzaTipoDocumento.Letra or CobranzaTipoDocumento.Cheque;

        public static string NormalizeType(string typeCode) => typeCode.Trim();

        public static string? PlantaOrNull(string typeCode, string? plantaCode)
            => UsaPlanta(typeCode) && !string.IsNullOrWhiteSpace(plantaCode) ? LetraVentaRules.Normalize(plantaCode) : null;

        public static string? BancoOrNull(string typeCode, string? bancoCode)
            => typeCode == CobranzaTipoDocumento.Cheque && !string.IsNullOrWhiteSpace(bancoCode)
                ? bancoCode.Trim().ToUpperInvariant()
                : null;

        public static string? ClienteOrNull(string? clienteCode)
            => string.IsNullOrWhiteSpace(clienteCode) ? null : clienteCode.Trim();

        /// <summary>Valida los datos que identifican al documento (planta para 08/10, cliente y banco para cheques).</summary>
        public static void EnsureReference(string typeCode, string number, string? plantaCode, string? clienteCode, string? bancoCode)
        {
            if (!TiposPermitidos.Contains(typeCode))
            {
                throw new InvalidOperationException(
                    typeCode == CobranzaTipoDocumento.NotaCredito
                        ? "La nota de crédito no se envía a cobranza dudosa."
                        : $"Tipo de documento {typeCode} no válido para cobranza dudosa (01, 03, 08, 10 u 11).");
            }

            if (UsaPlanta(typeCode) && PlantaOrNull(typeCode, plantaCode) is null)
            {
                throw new InvalidOperationException($"Indique la planta del documento {typeCode} {number}.");
            }

            if (typeCode == CobranzaTipoDocumento.Cheque
                && (ClienteOrNull(clienteCode) is null || BancoOrNull(typeCode, bancoCode) is null))
            {
                throw new InvalidOperationException($"Indique el cliente y el banco del cheque {number}.");
            }
        }

        /// <summary>
        /// Saldo del documento en soles y en dólares (legacy PendienteSol / PendienteDol). En documentos en moneda
        /// extranjera los soles son el saldo al tipo de cambio del documento. En documentos en soles los dólares son
        /// solo informativos (saldo al tipo de cambio de la cobranza dudosa).
        /// </summary>
        public static (decimal Soles, decimal Dollars) Totals(CobranzaDocumento doc, decimal exchangeRate)
        {
            if (doc.CurrencyCode == DocumentoVentaRules.MonedaBase)
            {
                return (doc.Pending, exchangeRate > 0 ? DocumentoVentaRules.Round2(doc.Pending / exchangeRate) : 0m);
            }

            return (DocumentoVentaRules.Round2(doc.Pending * doc.ExchangeRate), doc.Pending);
        }

        public static void EnsureRowVersion(CobranzaDudosa cd, byte[] rowVersion)
        {
            if (!cd.RowVersion.SequenceEqual(rowVersion))
            {
                throw new InvalidOperationException(
                    "La cobranza dudosa fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }
        }

        /// <summary>Busca el registro de cobranza dudosa (con seguimiento) por documento.</summary>
        public static async Task<CobranzaDudosa> GetTrackedAsync(
            IUnitOfWork uow, string typeCode, string? plantaCode, string number, string? clienteCode, string? bancoCode,
            CancellationToken cancellationToken)
        {
            var type = NormalizeType(typeCode);
            var num = number.Trim();
            var planta = PlantaOrNull(type, plantaCode) ?? string.Empty;
            var banco = BancoOrNull(type, bancoCode) ?? string.Empty;
            var cliente = ClienteOrNull(clienteCode);

            var q = uow.Facturacion.Transacciones.CobranzasDudosas.Query()
                .Where(x => x.TypeCode == type && x.PlantaCode == planta && x.Number == num && x.BancoCode == banco);

            if (cliente is not null)
            {
                q = q.Where(x => x.ClienteCode == cliente);
            }

            var found = await q.Take(2).ToListAsync(cancellationToken);

            if (found.Count == 0)
            {
                throw new KeyNotFoundException($"Cobranza dudosa del documento {type} {num} no registrada.");
            }

            if (found.Count > 1)
            {
                throw new InvalidOperationException($"El documento {type} {num} existe para más de un cliente: indique el cliente.");
            }

            return found[0];
        }

        /// <summary>Carga el documento de venta al que pertenece el registro.</summary>
        public static async Task<CobranzaDocumento> LoadDocumentAsync(
            IUnitOfWork uow, CobranzaDudosa cd, CancellationToken cancellationToken)
            => await CobranzaDocumentoService.LoadAsync(
                   uow, cd.TypeCode, string.IsNullOrEmpty(cd.PlantaCode) ? null : cd.PlantaCode, cd.Number, cd.ClienteCode,
                   string.IsNullOrEmpty(cd.BancoCode) ? null : cd.BancoCode, cancellationToken)
               ?? throw new KeyNotFoundException($"Documento {cd.TypeCode} {cd.Number} no encontrado.");
    }
}
