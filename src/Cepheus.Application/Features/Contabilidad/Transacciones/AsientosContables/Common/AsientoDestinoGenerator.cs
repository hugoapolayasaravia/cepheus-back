using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Maestros;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common
{
    /// <summary>
    /// Generación de las líneas de destino (legacy: co_genera_destino). Trabaja sobre un asiento NUEVO
    /// (todavía no guardado) y reconstruye su detalle:
    ///
    ///  1. Conserva solo las líneas normales (EsDestino = false).
    ///  2. Consolida las líneas iguales por cuenta + anexo + documento + planta (suma cargos y abonos).
    ///  3. Si la cuenta tiene sus dos cuentas de enlace (8 caracteres cada una), genera dos líneas de destino:
    ///       enlace 1: mismos cargos y abonos;  enlace 2: cargos y abonos invertidos.
    ///     Si la cuenta no tiene enlaces, la cuenta empieza con 9 y la línea trae un anexo de 5 caracteres,
    ///     se usan los enlaces del anexo.
    ///     Una cuenta de enlace que exige tipo de anexo (3 caracteres) toma el anexo de la línea.
    ///  4. Las líneas de destino también se consolidan por cuenta + anexo + documento + planta.
    ///  5. Recalcula los totales.
    ///
    /// Diferencias con el legacy (que insertaba sin FK): se exige que las cuentas de enlace existan y que el
    /// anexo exista en el tipo que pide la cuenta de enlace.
    /// </summary>
    public static class AsientoDestinoGenerator
    {
        public static async Task ApplyAsync(
            IUnitOfWork uow, AsientoContable asiento, CancellationToken cancellationToken)
        {
            var origenes = asiento.Detalles
                .Where(d => !d.EsDestino)
                .OrderBy(d => d.ItemNumber)
                .ToList();

            asiento.Detalles.Clear();

            var cuentaCache = new Dictionary<string, CuentaContable?>(StringComparer.Ordinal);
            var anexoCache = new Dictionary<string, Anexo?>(StringComparer.Ordinal);
            var resultado = new List<AsientoContableDetalle>();
            var item = 0;

            async Task<CuentaContable?> FindCuenta(string code)
            {
                if (!cuentaCache.TryGetValue(code, out var cuenta))
                {
                    cuenta = await uow.Contabilidad.Maestros.CuentasContables.Query()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);
                    cuentaCache[code] = cuenta;
                }

                return cuenta;
            }

            async Task<Anexo?> FindAnexo(string tipo, string code)
            {
                var key = tipo + "|" + code;
                if (!anexoCache.TryGetValue(key, out var anexo))
                {
                    anexo = await uow.Contabilidad.Maestros.Anexos.Query()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.TipoAnexoCode == tipo && a.Code == code, cancellationToken);
                    anexoCache[key] = anexo;
                }

                return anexo;
            }

            AsientoContableDetalle? Buscar(string cuenta, string anexo, string documento, string planta)
                => resultado.FirstOrDefault(l =>
                    l.CuentaContableCode == cuenta
                    && (l.AnexoCode ?? string.Empty) == anexo
                    && (l.NumeroDocumento ?? string.Empty) == documento
                    && l.PlantaCode == planta);

            AsientoContableDetalle Agregar(
                AsientoContableDetalle origen, string cuenta, string? anexoTipo, string? anexo,
                bool esDestino, decimal cargo, decimal abono)
            {
                item++;
                var linea = new AsientoContableDetalle
                {
                    Anio = asiento.Anio,
                    Mes = asiento.Mes,
                    TipoAsientoCode = asiento.TipoAsientoCode,
                    Numero = asiento.Numero,
                    ItemNumber = item,
                    PlantaCode = origen.PlantaCode,
                    EsDestino = esDestino,
                    Descripcion = origen.Descripcion,
                    CuentaContableCode = cuenta,
                    AnexoTipoCode = anexoTipo,
                    AnexoCode = anexo,
                    NumeroDocumento = origen.NumeroDocumento,
                    CargoSol = cargo,
                    AbonoSol = abono,
                    TipoContabilidad = origen.TipoContabilidad,
                    PlantaEjecutoraCode = origen.PlantaEjecutoraCode,
                    ComprobantePagoCode = origen.ComprobantePagoCode
                };
                resultado.Add(linea);
                return linea;
            }

            foreach (var src in origenes)
            {
                var ctaCode = src.CuentaContableCode.Trim();
                var cuenta = await FindCuenta(ctaCode)
                    ?? throw new InvalidOperationException($"Cuenta {ctaCode} no registrada ... Reintente ...");

                var ane = (src.AnexoCode ?? string.Empty).Trim();
                var doc = (src.NumeroDocumento ?? string.Empty).Trim();

                var en1 = (cuenta.LinkAccount1Code ?? string.Empty).Trim();
                var en2 = (cuenta.LinkAccount2Code ?? string.Empty).Trim();
                var conEnlaces = en1.Length == 8 && en2.Length == 8;

                if (!conEnlaces && ctaCode.StartsWith('9') && ane.Length > 0 && ane.Length < 5)
                {
                    throw new InvalidOperationException(
                        $"Anexo {ane} de la Cuenta {ctaCode} es inválido ... Reintente ...");
                }

                if (!conEnlaces && ctaCode.StartsWith('9') && ane.Length == 5)
                {
                    var tipoAnexo = (src.AnexoTipoCode ?? string.Empty).Trim();
                    var anexo = await FindAnexo(tipoAnexo, ane)
                        ?? throw new InvalidOperationException(
                            $"Anexo nro {ane} no existe en el tipo {tipoAnexo} del item {src.ItemNumber}.");

                    en1 = (anexo.LinkAccount1Code ?? string.Empty).Trim();
                    en2 = (anexo.LinkAccount2Code ?? string.Empty).Trim();
                    conEnlaces = en1.Length == 8 && en2.Length == 8;
                }

                // Línea normal, consolidada.
                var existente = Buscar(ctaCode, ane, doc, src.PlantaCode);
                if (existente is null)
                {
                    Agregar(src, ctaCode, src.AnexoTipoCode, src.AnexoCode, false, src.CargoSol, src.AbonoSol);
                }
                else
                {
                    existente.CargoSol += src.CargoSol;
                    existente.AbonoSol += src.AbonoSol;
                }

                if (!conEnlaces)
                {
                    continue;
                }

                async Task AgregarDestino(string cuentaEnlace, decimal cargo, decimal abono)
                {
                    var linkCuenta = await FindCuenta(cuentaEnlace)
                        ?? throw new InvalidOperationException($"No se registra cuenta {cuentaEnlace} de enlace ...");

                    var tipoAnexoEnlace = (linkCuenta.TipoAnexoCode ?? string.Empty).Trim();
                    var anexoDestino = tipoAnexoEnlace.Length == 3 ? ane : string.Empty;

                    if (anexoDestino.Length > 0 && await FindAnexo(tipoAnexoEnlace, anexoDestino) is null)
                    {
                        throw new InvalidOperationException(
                            $"El anexo {anexoDestino} no existe en el tipo {tipoAnexoEnlace} que exige la cuenta de enlace {cuentaEnlace}.");
                    }

                    var destino = Buscar(cuentaEnlace, anexoDestino, doc, src.PlantaCode);
                    if (destino is null)
                    {
                        Agregar(
                            src,
                            cuentaEnlace,
                            anexoDestino.Length > 0 ? tipoAnexoEnlace : null,
                            anexoDestino.Length > 0 ? anexoDestino : null,
                            true,
                            cargo,
                            abono);
                    }
                    else
                    {
                        destino.CargoSol += cargo;
                        destino.AbonoSol += abono;
                    }
                }

                await AgregarDestino(en1, src.CargoSol, src.AbonoSol);
                await AgregarDestino(en2, src.AbonoSol, src.CargoSol);
            }

            foreach (var linea in resultado)
            {
                asiento.Detalles.Add(linea);
            }

            AsientoCuadre.Totalize(asiento);
        }
    }
}
