using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Maestros.PeriodosContables.Common;
using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;
using Cepheus.Domain.Logistica.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common
{
    public static class PagoProveedorRules
    {
        /// <summary>Todo el pago es en soles (indicación del usuario: sin dólares).</summary>
        public const string MonedaBase = "PEN";
        public const int CodeLength = 10;
        public const int AsientoLength = 13;

        /// <summary>Nota de crédito: en el pago sus montos van en negativo (legacy ue_carga_detalle).</summary>
        public const string NotaCredito = "07";

        /// <summary>NroControl_che '*' = operación sin cheque físico (no consume la chequera).</summary>
        public const string SinCheque = "*";

        /// <summary>COD_FOR_PAG_CANCELACION '3': pago a la cuenta bancaria del proveedor.</summary>
        public const string FormaPagoCuentaProveedor = "3";

        // Legacy: match('01*08*07', tipo) -> factura, nota de débito y nota de crédito.
        private static readonly string[] ComprobantesRetencion = { "01", "08", "07" };

        public static string Normalize(string value) => value.Trim().ToUpperInvariant();

        public static bool EsComprobanteDeRetencion(string comprobantePagoCode)
            => ComprobantesRetencion.Contains(comprobantePagoCode);

        public static int Sign(string comprobantePagoCode)
            => comprobantePagoCode == NotaCredito ? -1 : 1;

        /// <summary>Legacy ue_carga_detalle: el documento tiene detracción si registra número de detracción.</summary>
        public static bool TieneDetraccion(DocumentoCompra doc)
            => !string.IsNullOrWhiteSpace(doc.NumeroDetraccion);

        /// <summary>Legacy fg_cierre: la fecha no puede caer en un período contable cerrado.</summary>
        public static Task EnsurePeriodOpenAsync(IUnitOfWork uow, DateTime date, CancellationToken cancellationToken)
            => PeriodoContableRules.EnsureOpenAsync(
                uow, date.Year.ToString("0000"), date.Month.ToString("00"), cancellationToken);

        /// <summary>Legacy: solo se edita un pago Pendiente sin asiento de cancelación generado.</summary>
        public static void EnsureEditable(PagoProveedor pago)
        {
            if (pago.Estado != EstadoPagoProveedor.Pendiente)
            {
                throw new InvalidOperationException(
                    $"El pago {pago.Code} está en estado '{pago.Estado}' y ya no admite cambios.");
            }

            if (!string.IsNullOrWhiteSpace(pago.AsientoCancelacionCode))
            {
                throw new InvalidOperationException(
                    $"El asiento de cancelación {pago.AsientoCancelacionCode} ya fue generado.");
            }
        }

        /// <summary>
        /// Legacy: correlativo de Codigo_che (max + 1, 10 dígitos). No se usa SequentialCodeGenerator
        /// porque su validación de máximo usa int y no soporta 10 dígitos. Los códigos tienen ancho
        /// fijo, por lo que el mayor por orden alfabético es el mayor numérico.
        /// </summary>
        public static async Task<string> NextCodeAsync(IUnitOfWork uow, CancellationToken cancellationToken)
        {
            var last = await uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .AsNoTracking()
                .OrderByDescending(p => p.Code)
                .Select(p => p.Code)
                .FirstOrDefaultAsync(cancellationToken);

            long max = 0;
            if (last is not null && long.TryParse(last, out var parsed))
            {
                max = parsed;
            }

            var next = max + 1;
            if (next > 9_999_999_999L)
            {
                throw new InvalidOperationException(
                    "No hay códigos disponibles para Pagos a Proveedores: se alcanzó el máximo de 10 dígitos.");
            }

            return next.ToString().PadLeft(CodeLength, '0');
        }

        /// <summary>
        /// Legacy fg_afecto: el proveedor es afecto a retención salvo que su RUC figure en la tabla
        /// de no afectos (cualquier Tipo). Solo cuentan los registros activos.
        /// </summary>
        public static async Task<bool> IsAfectoAsync(
            IUnitOfWork uow, Proveedor proveedor, CancellationToken cancellationToken)
        {
            var ruc = proveedor.DocumentNumber?.Trim();
            if (string.IsNullOrEmpty(ruc))
            {
                return true;
            }

            var noAfecto = await uow.Contabilidad.Maestros.NoAfectosRetencion.Query()
                .AsNoTracking()
                .AnyAsync(n => n.Ruc == ruc && n.IsActive, cancellationToken);

            return !noAfecto;
        }

        /// <summary>
        /// Valida y aplica la cabecera del pago (legacy ue_set_btn_grb_data, parte cabecera) sobre la
        /// entidad. Actualiza también el último cheque usado de la cuenta bancaria (legacy:
        /// RCtaBancos.cheactual_cta) cuando corresponde.
        /// </summary>
        public static async Task ApplyCabeceraAsync(
            IUnitOfWork uow,
            PagoProveedor pago,
            IPagoProveedorCabecera input,
            bool allowChangeProveedorYFecha,
            CancellationToken cancellationToken)
        {
            var prvCode = Normalize(input.ProveedorCode);
            var fecha = input.FechaEmision.Date;

            if (!allowChangeProveedorYFecha &&
                (pago.ProveedorCode != prvCode || pago.FechaEmision.Date != fecha))
            {
                throw new InvalidOperationException(
                    "El pago ya tiene documentos: no se puede cambiar el proveedor ni la fecha de emisión.");
            }

            var proveedor = await uow.Logistica.Maestros.Proveedores.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Code == prvCode, cancellationToken);

            if (proveedor is null)
            {
                throw new KeyNotFoundException($"Proveedor {prvCode} no encontrado.");
            }

            if (string.IsNullOrWhiteSpace(proveedor.DocumentNumber))
            {
                throw new InvalidOperationException("Número de RUC errado: el proveedor no registra RUC.");
            }

            await EnsurePeriodOpenAsync(uow, fecha, cancellationToken);

            var banco = Normalize(input.BancoCode);
            var numeroCuenta = input.CuentaBancariaNumero.Trim();

            var cuenta = await uow.Contabilidad.Maestros.CuentasBancarias.Query()
                .FirstOrDefaultAsync(c => c.BankCode == banco && c.AccountNumber == numeroCuenta, cancellationToken);

            if (cuenta is null)
            {
                throw new KeyNotFoundException($"Cuenta bancaria {banco}/{numeroCuenta} no encontrada.");
            }

            if (!cuenta.IsActive)
            {
                throw new InvalidOperationException("La cuenta bancaria está inactiva.");
            }

            if (cuenta.CurrencyCode.Trim() != MonedaBase)
            {
                throw new InvalidOperationException("La cuenta bancaria debe ser en soles.");
            }

            var planta = Normalize(input.PlantaCode);
            if (!await uow.Comunes.Plantas.Query().AsNoTracking().AnyAsync(p => p.Code == planta, cancellationToken))
            {
                throw new KeyNotFoundException($"Planta {planta} no encontrada.");
            }

            var forma = input.FormaPagoCancelacionCode.Trim();
            var formaPago = await uow.Contabilidad.Catalogos.FormasPagoCancelacion.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Code == forma, cancellationToken);

            if (formaPago is null)
            {
                throw new KeyNotFoundException($"Forma de pago {forma} no encontrada.");
            }

            if (!formaPago.IsActive)
            {
                throw new InvalidOperationException($"La forma de pago {forma} está inactiva.");
            }

            int? proveedorCuentaId = null;
            if (forma == FormaPagoCuentaProveedor)
            {
                var id = input.ProveedorCuentaId!.Value;
                var cuentaProveedor = await uow.Logistica.Maestros.ProveedorCuentas.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id && c.ProveedorCode == prvCode, cancellationToken);

                if (cuentaProveedor is null)
                {
                    throw new KeyNotFoundException($"Cuenta bancaria {id} del proveedor {prvCode} no encontrada.");
                }

                if (!cuentaProveedor.IsActive)
                {
                    throw new InvalidOperationException("La cuenta bancaria del proveedor está inactiva.");
                }

                if (cuentaProveedor.BancoCode != banco || cuentaProveedor.MonedaCode != MonedaBase)
                {
                    throw new InvalidOperationException(
                        "La cuenta del proveedor debe ser del mismo banco del pago y en soles.");
                }

                proveedorCuentaId = cuentaProveedor.Id;
            }

            pago.ProveedorCode = prvCode;
            pago.FechaEmision = fecha;
            pago.MonedaCode = MonedaBase;
            pago.BancoCode = banco;
            pago.CuentaBancariaNumero = numeroCuenta;
            pago.CuentaContableCode = cuenta.AccountCode;
            pago.NumeroCheque = (input.NumeroCheque ?? string.Empty).Trim();
            pago.NumeroControl = input.NumeroControl.Trim();
            pago.PlantaCode = planta;
            pago.Glosa = (input.Glosa ?? string.Empty).Trim();
            pago.Canje = input.Canje;
            pago.DepositoElectronico = input.DepositoElectronico;
            pago.FormaPagoCancelacionCode = forma;
            pago.ProveedorCuentaId = proveedorCuentaId;
            pago.Afecto = await IsAfectoAsync(uow, proveedor, cancellationToken);

            var pagarA = string.IsNullOrWhiteSpace(input.PagarA) ? proveedor.LegalName : input.PagarA;
            pago.PagarA = pagarA.Trim().Length > 150 ? pagarA.Trim()[..150] : pagarA.Trim();

            // Legacy: RCtaBancos.cheactual_cta = número del cheque emitido (solo cheques físicos).
            if (pago.NumeroControl != SinCheque && !pago.Canje)
            {
                cuenta.CheckCurrent = pago.NumeroCheque;
            }
        }
    }
}
