using System.Text.RegularExpressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common
{
    public static class NotaDebitoVentaRules
    {
        public const int MaxLines = 999;
        private const int MaxAttempts = 3;
        private const string InitialNumber = "000-00000";

        /// <summary>Código legacy de "sin documento" en Codigo_tdo.</summary>
        public const string SinDocumento = "00";

        public static string Normalize(string value) => value.Trim().ToUpperInvariant();

        public static string FormatKey(string plantaCode, string number) => $"{plantaCode}/{number}";

        /// <summary>
        /// Siguiente número a partir del último emitido por la planta (Planta.NumeroDve):
        /// conserva el prefijo y los ceros ("000-00009" -> "000-00010").
        /// </summary>
        public static string IncrementNumber(string? current)
        {
            var text = string.IsNullOrWhiteSpace(current) ? InitialNumber : current.Trim();
            var match = Regex.Match(text, @"^(?<prefix>.*?)(?<num>\d+)$");

            if (!match.Success)
            {
                throw new InvalidOperationException(
                    $"La numeración de la planta ({text}) no tiene un formato válido (ej. {InitialNumber}).");
            }

            var digits = match.Groups["num"].Value;
            var next = (long.Parse(digits) + 1).ToString().PadLeft(digits.Length, '0');

            if (next.Length > digits.Length)
            {
                throw new InvalidOperationException("Se agotó la numeración de notas de débito de la planta.");
            }

            return match.Groups["prefix"].Value + next;
        }

        /// <summary>
        /// Toma el siguiente número de la planta, arma la nota con <paramref name="factory"/> y la
        /// graba junto con la actualización de Planta.NumeroDve en una sola operación.
        /// Si otro usuario toma el número en paralelo, reintenta.
        /// </summary>
        public static async Task<NotaDebitoVenta> SaveNewAsync(
            IUnitOfWork uow, string plantaCode,
            Func<string, Task<NotaDebitoVenta>> factory,
            CancellationToken cancellationToken)
        {
            for (var attempt = 1; ; attempt++)
            {
                var planta = await uow.Comunes.Plantas.Query()
                    .FirstOrDefaultAsync(p => p.Code == plantaCode, cancellationToken);

                if (planta is null)
                {
                    throw new KeyNotFoundException($"Planta {plantaCode} no encontrada.");
                }

                var number = IncrementNumber(planta.NumeroDve);

                // Salvaguarda ante numeración desfasada respecto a notas ya registradas.
                for (var guard = 0; guard < 100; guard++)
                {
                    var n = number;
                    var taken = await uow.Facturacion.Transacciones.NotasDebitoVenta.Query()
                        .AnyAsync(x => x.PlantaCode == plantaCode && x.Number == n, cancellationToken);

                    if (!taken)
                    {
                        break;
                    }

                    number = IncrementNumber(number);
                }

                planta.NumeroDve = number;

                var nota = await factory(number);

                await uow.Facturacion.Transacciones.NotasDebitoVenta.AddAsync(nota, cancellationToken);

                try
                {
                    await uow.SaveChangesAsync(cancellationToken);
                    return nota;
                }
                catch (DbUpdateException) when (attempt < MaxAttempts)
                {
                    uow.ClearTracking();
                }
            }
        }

        public static async Task<Dictionary<(string Tipo, string Producto), string>> LoadProductNamesAsync(
            IUnitOfWork uow, IEnumerable<NotaDebitoVentaDetalleInput> inputs, CancellationToken cancellationToken)
        {
            var names = new Dictionary<(string Tipo, string Producto), string>();

            foreach (var (tipo, producto) in inputs
                .Select(i => (Tipo: i.TipoProductoCode.Trim(), Producto: i.ProductoCode.Trim()))
                .Distinct())
            {
                var t = tipo;
                var p = producto;

                var name = await uow.Facturacion.Maestros.Productos.Query()
                    .AsNoTracking()
                    .Where(x => x.TipoProductoCode == t && x.Code == p)
                    .Select(x => x.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                if (name is null)
                {
                    throw new KeyNotFoundException($"Producto {t}-{p} no encontrado.");
                }

                names[(t, p)] = name;
            }

            return names;
        }

        public static (List<NotaDebitoVentaDetalle> Details, decimal SubTotal) BuildDetails(
            string plantaCode, string number,
            IReadOnlyList<NotaDebitoVentaDetalleInput> inputs,
            IReadOnlyDictionary<(string Tipo, string Producto), string> productNames)
        {
            var details = new List<NotaDebitoVentaDetalle>(inputs.Count);

            for (var i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                var lineNumber = i + 1;
                var tipo = input.TipoProductoCode.Trim();
                var producto = input.ProductoCode.Trim();

                details.Add(new NotaDebitoVentaDetalle
                {
                    PlantaCode = plantaCode,
                    Number = number,
                    Line = lineNumber.ToString().PadLeft(3, '0'),
                    TipoProductoCode = tipo,
                    ProductoCode = producto,
                    Description = string.IsNullOrWhiteSpace(input.Description)
                        ? productNames[(tipo, producto)]
                        : input.Description.Trim(),
                    Quantity = input.Quantity,
                    UnitPrice = input.UnitPrice,
                    Total = DocumentoVentaRules.Round2(input.Quantity * input.UnitPrice),
                    LineKey = input.LineKey ?? lineNumber
                });
            }

            return (details, details.Sum(d => d.Total));
        }

        /// <summary>
        /// Reemplaza las líneas de la nota por número de línea (actualiza, agrega y quita) sin borrar y
        /// reinsertar la misma clave.
        /// </summary>
        public static void ReplaceDetails(NotaDebitoVenta nota, IReadOnlyList<NotaDebitoVentaDetalle> fresh)
        {
            var existing = nota.Details.OrderBy(d => d.Line).ToList();

            for (var i = 0; i < fresh.Count; i++)
            {
                if (i < existing.Count)
                {
                    var e = existing[i];
                    e.TipoProductoCode = fresh[i].TipoProductoCode;
                    e.ProductoCode = fresh[i].ProductoCode;
                    e.Description = fresh[i].Description;
                    e.Quantity = fresh[i].Quantity;
                    e.UnitPrice = fresh[i].UnitPrice;
                    e.Total = fresh[i].Total;
                    e.LineKey = fresh[i].LineKey;
                }
                else
                {
                    nota.Details.Add(fresh[i]);
                }
            }

            for (var i = fresh.Count; i < existing.Count; i++)
            {
                nota.Details.Remove(existing[i]);
            }
        }

        public static async Task<Obra> GetObraAsync(
            IUnitOfWork uow, string clienteCode, string obraCode, CancellationToken cancellationToken)
        {
            var obra = await uow.Facturacion.Maestros.Obras.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.ClienteCode == clienteCode && o.Code == obraCode, cancellationToken);

            if (obra is null)
            {
                throw new KeyNotFoundException($"Obra {obraCode} del cliente {clienteCode} no encontrada.");
            }

            return obra;
        }

        /// <summary>
        /// Valida el documento que se debita (legacy: ue_validadocumento): sin documento (null o "00"),
        /// factura (01) o boleta (03) del mismo cliente.
        /// Devuelve el par normalizado (null, null) cuando no hay documento.
        /// </summary>
        public static async Task<(string? Type, string? Number)> ResolveReferenceAsync(
            IUnitOfWork uow, string clienteCode, string? documentTypeCode, string? documentNumber,
            CancellationToken cancellationToken)
        {
            var type = string.IsNullOrWhiteSpace(documentTypeCode) ? null : documentTypeCode.Trim();
            var number = string.IsNullOrWhiteSpace(documentNumber) ? null : documentNumber.Trim();

            if (type is null || type == SinDocumento)
            {
                if (number is not null)
                {
                    throw new InvalidOperationException(
                        "Si la nota de débito no tiene documento no se indica número de documento.");
                }

                return (null, null);
            }

            if (number is null)
            {
                throw new InvalidOperationException("Debe indicar el número del documento que se debita.");
            }

            if (type != "01" && type != "03")
            {
                throw new InvalidOperationException(
                    $"El tipo de documento {type} no está habilitado para la nota de débito (01 factura, 03 boleta o 00 sin documento).");
            }

            var comprobante = await uow.Facturacion.Transacciones.ComprobantesVenta.Query()
                .AsNoTracking()
                .Where(c => c.TypeCode == type && c.Number == number)
                .Select(c => new { c.ClienteCode })
                .FirstOrDefaultAsync(cancellationToken);

            if (comprobante is null)
            {
                var label = type == "01" ? "La factura" : "La boleta";
                throw new KeyNotFoundException($"{label} {number} no existe.");
            }

            if (comprobante.ClienteCode != clienteCode)
            {
                throw new InvalidOperationException("El documento no corresponde al cliente.");
            }

            return (type, number);
        }
    }
}
