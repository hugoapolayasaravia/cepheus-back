using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common
{
    /// <summary>
    /// Motor de plantillas: arma las líneas de un asiento NUEVO a partir de una plantilla (PlantillaAsiento) y de
    /// los datos del documento (AsientoFuente). Reemplaza a las cuentas escritas en el programa legacy.
    ///
    /// Reglas del legacy que conserva:
    ///   - Las líneas de IGV, IGV exterior, renta, fonavi, no gravable y servicio solo se generan si el importe es
    ///     mayor a 0; el total y el detalle siempre se generan.
    ///   - Si la plantilla no tiene una línea de concepto Servicio, el servicio se suma a la primera línea del
    ///     detalle del documento.
    ///   - En nota de crédito, las líneas con InvertsOnCreditNote cambian de naturaleza.
    ///   - El anexo "Proveedor" solo se informa si la cuenta exige tipo de anexo.
    /// </summary>
    public static class AsientoPlantillaMotor
    {
        private static readonly ConceptoAsiento[] ConceptosOpcionales =
        {
            ConceptoAsiento.Igv, ConceptoAsiento.IgvExterior, ConceptoAsiento.Renta,
            ConceptoAsiento.Fonavi, ConceptoAsiento.NoGravable, ConceptoAsiento.Servicio
        };

        public static async Task BuildAsync(
            IUnitOfWork uow,
            PlantillaAsiento plantilla,
            AsientoFuente fuente,
            AsientoContable asiento,
            CancellationToken cancellationToken)
        {
            var cuentaCache = new Dictionary<string, Cepheus.Domain.Contabilidad.Maestros.CuentaContable?>(StringComparer.Ordinal);
            var anexoCache = new Dictionary<string, bool>(StringComparer.Ordinal);
            var tieneLineaServicio = plantilla.Lines.Any(l => l.Concept == ConceptoAsiento.Servicio);
            var item = asiento.Detalles.Count;

            async Task<Cepheus.Domain.Contabilidad.Maestros.CuentaContable> GetCuenta(string code)
            {
                if (!cuentaCache.TryGetValue(code, out var cuenta))
                {
                    cuenta = await uow.Contabilidad.Maestros.CuentasContables.Query()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
                    cuentaCache[code] = cuenta;
                }

                return cuenta ?? throw new InvalidOperationException($"Cuenta {code} no registrada ... Reintente ...");
            }

            async Task EnsureAnexoAsync(string tipo, string code)
            {
                var key = tipo + "|" + code;
                if (!anexoCache.TryGetValue(key, out var existe))
                {
                    existe = await uow.Contabilidad.Maestros.Anexos.Query()
                        .AsNoTracking()
                        .AnyAsync(a => a.TipoAnexoCode == tipo && a.Code == code, cancellationToken);
                    anexoCache[key] = existe;
                }

                if (!existe)
                {
                    throw new InvalidOperationException($"El anexo {code} no está registrado en el tipo de anexo {tipo}.");
                }
            }

            async Task AddLineAsync(
                PlantillaAsientoLinea tl, NaturalezaAsiento naturaleza, decimal importe, AsientoFuenteLinea? linea)
            {
                var cuentaCode = tl.AccountSource switch
                {
                    OrigenCuentaAsiento.Fija => (tl.AccountCode ?? string.Empty).Trim(),
                    OrigenCuentaAsiento.CuentaPorPagarProveedor => (fuente.CuentaPorPagarCode ?? string.Empty).Trim(),
                    OrigenCuentaAsiento.LineaDocumento => (linea?.CuentaCode ?? string.Empty).Trim(),
                    _ => string.Empty
                };

                if (cuentaCode.Length == 0)
                {
                    throw new InvalidOperationException(
                        $"La línea {tl.Order} de la plantilla {plantilla.Code} no pudo resolver su cuenta contable.");
                }

                var cuenta = await GetCuenta(cuentaCode);

                string? anexoTipo = null;
                string? anexoCode = null;

                if (tl.AnnexSource == OrigenAnexoAsiento.Proveedor)
                {
                    var tipoCuenta = (cuenta.TipoAnexoCode ?? string.Empty).Trim();
                    if (tipoCuenta.Length == 3 && !string.IsNullOrWhiteSpace(fuente.ProveedorCode))
                    {
                        anexoTipo = tipoCuenta;
                        anexoCode = fuente.ProveedorCode!.Trim();
                    }
                }
                else if (tl.AnnexSource == OrigenAnexoAsiento.LineaDocumento
                         && !string.IsNullOrWhiteSpace(linea?.AnexoTipoCode)
                         && !string.IsNullOrWhiteSpace(linea?.AnexoCode))
                {
                    anexoTipo = linea!.AnexoTipoCode!.Trim();
                    anexoCode = linea.AnexoCode!.Trim();
                }

                if (anexoTipo is not null && anexoCode is not null)
                {
                    await EnsureAnexoAsync(anexoTipo, anexoCode);
                }

                var descripcion = tl.DescriptionSource switch
                {
                    OrigenDescripcionAsiento.NombreProveedor => fuente.ProveedorNombre ?? fuente.Glosa,
                    OrigenDescripcionAsiento.NumeroDocumento => fuente.NumeroDocumento ?? fuente.Glosa,
                    OrigenDescripcionAsiento.DetalleLinea => linea?.Detalle ?? fuente.Glosa,
                    _ => fuente.Glosa
                };

                item++;
                asiento.Detalles.Add(new AsientoContableDetalle
                {
                    Anio = asiento.Anio,
                    Mes = asiento.Mes,
                    TipoAsientoCode = asiento.TipoAsientoCode,
                    Numero = asiento.Numero,
                    ItemNumber = item,
                    PlantaCode = linea?.PlantaCode ?? fuente.PlantaCode,
                    EsDestino = tl.IsDestination,
                    Descripcion = AsientoNotaIngresoRules.Truncate(descripcion, AsientoNotaIngresoRules.DescripcionLineaMax),
                    CuentaContableCode = cuentaCode,
                    AnexoTipoCode = anexoTipo,
                    AnexoCode = anexoCode,
                    NumeroDocumento = string.IsNullOrWhiteSpace(fuente.NumeroDocumento) ? null : fuente.NumeroDocumento.Trim(),
                    CargoSol = naturaleza == NaturalezaAsiento.Debe ? importe : 0m,
                    AbonoSol = naturaleza == NaturalezaAsiento.Haber ? importe : 0m,
                    TipoContabilidad = tl.AccountingType,
                    PlantaEjecutoraCode = fuente.PlantaCode,
                    ComprobantePagoCode = fuente.ComprobantePagoCode
                });
            }

            foreach (var tl in plantilla.Lines.OrderBy(l => l.Order))
            {
                var naturaleza = tl.Nature;
                if (tl.InvertsOnCreditNote && fuente.EsNotaCredito)
                {
                    naturaleza = naturaleza == NaturalezaAsiento.Debe ? NaturalezaAsiento.Haber : NaturalezaAsiento.Debe;
                }

                if (tl.Concept == ConceptoAsiento.DetalleDocumento)
                {
                    for (var i = 0; i < fuente.Lineas.Count; i++)
                    {
                        var linea = fuente.Lineas[i];
                        var importe = linea.Parcial;
                        if (i == 0 && !tieneLineaServicio)
                        {
                            importe += fuente.Servicio;
                        }

                        await AddLineAsync(tl, naturaleza, importe, linea);
                    }

                    continue;
                }

                var monto = tl.Concept switch
                {
                    ConceptoAsiento.TotalDocumento => fuente.Total,
                    ConceptoAsiento.Igv => fuente.Igv,
                    ConceptoAsiento.IgvExterior => fuente.IgvExterior,
                    ConceptoAsiento.Renta => fuente.Renta,
                    ConceptoAsiento.Fonavi => fuente.Fonavi,
                    ConceptoAsiento.NoGravable => fuente.NoGravable,
                    ConceptoAsiento.Servicio => fuente.Servicio,
                    ConceptoAsiento.MontoNotaIngreso => fuente.MontoNotaIngreso,
                    ConceptoAsiento.NetoNotaIngreso => fuente.NetoNotaIngreso,
                    _ => 0m
                };

                if (ConceptosOpcionales.Contains(tl.Concept) && monto <= 0)
                {
                    continue;
                }

                await AddLineAsync(tl, naturaleza, monto, null);
            }

            AsientoCuadre.Totalize(asiento);
        }
    }
}
