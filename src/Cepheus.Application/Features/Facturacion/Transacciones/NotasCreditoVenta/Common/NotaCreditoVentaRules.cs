using System.Text.RegularExpressions;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Domain.Facturacion.Maestros;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common
{
    public static class NotaCreditoVentaRules
    {
        public const int MaxLines = 999;
        private const int MaxAttempts = 3;
        private const string InitialNumber = "000-00000";

        /// <summary>Valores de Tipo_cve del legacy (significado no documentado).</summary>
        public static readonly string[] AllowedTypes = { "N", "O", "P", "C" };

        public static string Normalize(string value) => value.Trim().ToUpperInvariant();

        public static string FormatKey(string plantaCode, string number) => $"{plantaCode}/{number}";

        /// <summary>
        /// Siguiente número a partir del último emitido por la planta (Planta.NumeroCve):
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
                throw new InvalidOperationException("Se agotó la numeración de notas de crédito de la planta.");
            }

            return match.Groups["prefix"].Value + next;
        }

        /// <summary>
        /// Toma el siguiente número de la planta, arma la nota con <paramref name="factory"/> y la
        /// graba junto con la actualización de Planta.NumeroCve en una sola operación.
        /// Si otro usuario toma el número en paralelo, reintenta (la fábrica se vuelve a ejecutar
        /// con el seguimiento de cambios limpio).
        /// </summary>
        public static async Task<NotaCreditoVenta> SaveNewAsync(
            IUnitOfWork uow, string plantaCode,
            Func<string, Task<NotaCreditoVenta>> factory,
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

                var number = IncrementNumber(planta.NumeroCve);

                // Salvaguarda ante numeración desfasada respecto a notas ya registradas.
                for (var guard = 0; guard < 100; guard++)
                {
                    var n = number;
                    var taken = await uow.Facturacion.Transacciones.NotasCreditoVenta.Query()
                        .AnyAsync(x => x.PlantaCode == plantaCode && x.Number == n, cancellationToken);

                    if (!taken)
                    {
                        break;
                    }

                    number = IncrementNumber(number);
                }

                planta.NumeroCve = number;

                var nota = await factory(number);

                await uow.Facturacion.Transacciones.NotasCreditoVenta.AddAsync(nota, cancellationToken);

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
            IUnitOfWork uow, IEnumerable<NotaCreditoVentaDetalleInput> inputs, CancellationToken cancellationToken)
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

        public static (List<NotaCreditoVentaDetalle> Details, decimal SubTotal) BuildDetails(
            string plantaCode, string number,
            IReadOnlyList<NotaCreditoVentaDetalleInput> inputs,
            IReadOnlyDictionary<(string Tipo, string Producto), string> productNames)
        {
            var details = new List<NotaCreditoVentaDetalle>(inputs.Count);

            for (var i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                var lineNumber = i + 1;
                var tipo = input.TipoProductoCode.Trim();
                var producto = input.ProductoCode.Trim();

                details.Add(new NotaCreditoVentaDetalle
                {
                    PlantaCode = plantaCode,
                    Number = number,
                    Line = lineNumber.ToString().PadLeft(3, '0'),
                    GuideNumber = string.IsNullOrWhiteSpace(input.GuideNumber) ? null : input.GuideNumber.Trim(),
                    TipoProductoCode = tipo,
                    ProductoCode = producto,
                    Description = string.IsNullOrWhiteSpace(input.Description)
                        ? productNames[(tipo, producto)]
                        : input.Description.Trim(),
                    Quantity = input.Quantity,
                    DispatchedQuantity = 0m,
                    UnitPrice = input.UnitPrice,
                    Total = DocumentoVentaRules.Round2(input.Quantity * input.UnitPrice),
                    LineKey = input.LineKey ?? lineNumber,
                    OrderCode = string.IsNullOrWhiteSpace(input.OrderCode) ? null : input.OrderCode.Trim()
                });
            }

            return (details, details.Sum(d => d.Total));
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
    }
}
