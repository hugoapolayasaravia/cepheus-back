using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Facturacion.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CobranzasDudosas.Common
{
    /// <summary>
    /// Asiento de canje a cobranza dudosa (legacy: fc_cobranzadudosacanje + co_genera_destino), armado con la plantilla
    /// activa del proceso CanjeCobranzaDudosa.
    ///
    /// Datos que calcula (la plantilla define cuentas, naturaleza y tipo de asiento):
    ///   - DetalleDocumento: la primera línea de la provisión con cuenta 121 o 123 al Debe y en la moneda del documento
    ///     (se prefiere la del anexo del cliente); va al Haber por el saldo en soles (TotalSoles).
    ///   - CobranzaDudosa: el saldo al tipo de cambio de la cobranza dudosa (en soles si el documento es en soles).
    ///     La plantilla pone las líneas (legacy: 12910101 y 94110107 al Debe, 19210101 al Haber).
    ///   - DiferenciaCambioGanancia / DiferenciaCambioPerdida: lo que falte para cuadrar entre Debe y Haber
    ///     (legacy: gs_ctadiferenciag 77610101 / gs_ctadiferenciap 96110103).
    /// El anexo de las cuentas fijas es el cliente, solo si la cuenta exige tipo de anexo. Se generan los destinos
    /// de las cuentas con enlace (legacy co_genera_destino). El tipo contable elegido en la cobranza dudosa se aplica
    /// a todas las líneas. No guarda: quien llama hace el SaveChanges.
    /// </summary>
    public static class CobranzaDudosaAsientoService
    {
        public static async Task<AsientoContable> BuildAsync(
            IUnitOfWork uow, CobranzaDudosa cd, CancellationToken cancellationToken)
        {
            if (!CancelacionVentaRules.IsAsiento(cd.ProvisionEntry))
            {
                throw new InvalidOperationException($"Asiento de provisión {cd.ProvisionEntry} incorrecto.");
            }

            var plantilla = await AsientoVentaService.GetPlantillaAsync(uow, ProcesoAsiento.CanjeCobranzaDudosa, cancellationToken);

            var provisionCode = cd.ProvisionEntry!.Trim();
            var (anio, mes, tipo, numero) = AsientoContableRules.ParseCodigo(provisionCode);

            var provision = await uow.Contabilidad.Transacciones.AsientosContables.Query()
                .AsNoTracking()
                .Include(x => x.Detalles)
                .FirstOrDefaultAsync(
                    x => x.Anio == anio && x.Mes == mes && x.TipoAsientoCode == tipo && x.Numero == numero,
                    cancellationToken)
                ?? throw new InvalidOperationException($"No se encontró el asiento de provisión {provisionCode}.");

            // Cuenta 12: primera línea 121 / 123 al Debe, en la moneda del documento.
            var candidatas = provision.Detalles
                .Where(d => d.CargoSol > 0
                            && (d.CuentaContableCode.StartsWith("121") || d.CuentaContableCode.StartsWith("123")))
                .OrderBy(d => d.ItemNumber)
                .ToList();

            var codigos = candidatas.Select(d => d.CuentaContableCode).Distinct().ToList();
            var enMoneda = await uow.Contabilidad.Maestros.CuentasContables.Query().AsNoTracking()
                .Where(c => codigos.Contains(c.Code) && c.CurrencyCode == cd.CurrencyCode)
                .Select(c => c.Code)
                .ToListAsync(cancellationToken);

            var validas = candidatas.Where(d => enMoneda.Contains(d.CuentaContableCode)).ToList();

            var linea12 = validas.FirstOrDefault(d => d.AnexoCode == cd.ClienteCode) ?? validas.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    $"Cuenta 12 no registrada en el asiento de provisión {provisionCode} para la moneda {cd.CurrencyCode}.");

            // Importes: saldo y saldo a tipo de cambio de la cobranza dudosa (X).
            var mos = cd.TotalSoles;
            var x = cd.CurrencyCode == DocumentoVentaRules.MonedaBase
                ? mos
                : DocumentoVentaRules.Round2(cd.TotalDollars * cd.ExchangeRate);

            // Diferencia de cambio: lo que falta para cuadrar según las líneas de la plantilla.
            var dudosaDebe = plantilla.Lines.Count(l => l.Concept == ConceptoAsiento.CobranzaDudosa && l.Nature == NaturalezaAsiento.Debe);
            var dudosaHaber = plantilla.Lines.Count(l => l.Concept == ConceptoAsiento.CobranzaDudosa && l.Nature == NaturalezaAsiento.Haber);

            var cargos = DocumentoVentaRules.Round2(dudosaDebe * x);
            var abonos = DocumentoVentaRules.Round2(mos + dudosaHaber * x);
            var diferencia = cargos - abonos;

            var fuente = new AsientoFuente
            {
                PlantaCode = linea12.PlantaCode,
                Glosa = linea12.Descripcion,
                NumeroDocumento = cd.Number,
                MonedaCode = cd.CurrencyCode,
                ClienteCode = cd.ClienteCode,
                CobranzaDudosa = x,
                DiferenciaGanancia = diferencia > 0 ? diferencia : 0m,
                DiferenciaPerdida = diferencia < 0 ? -diferencia : 0m,
                Lineas = new List<AsientoFuenteLinea>
                {
                    new()
                    {
                        PlantaCode = linea12.PlantaCode,
                        Detalle = linea12.Descripcion,
                        CuentaCode = linea12.CuentaContableCode,
                        AnexoTipoCode = linea12.AnexoTipoCode,
                        AnexoCode = linea12.AnexoCode,
                        Parcial = mos,
                        NumeroDocumento = cd.Number
                    }
                }
            };

            var asiento = await AsientoVentaService.BuildAsync(
                uow, plantilla, new[] { fuente }, cd.ProcessDate, $"COBRANZA DUDOSA {cd.TypeCode} {cd.Number}",
                cd.ExchangeRate, aplicarDestinos: true, cancellationToken);

            // Legacy: el tipo contable de la ventana va a todas las líneas (Tipo_con).
            foreach (var d in asiento.Detalles)
            {
                d.TipoContabilidad = cd.TipoContabilidad;
            }

            return asiento;
        }
    }
}
