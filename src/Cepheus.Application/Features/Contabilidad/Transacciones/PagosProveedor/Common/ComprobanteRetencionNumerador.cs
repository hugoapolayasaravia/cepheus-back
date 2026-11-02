using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common
{
    /// <summary>
    /// Legacy (ue_set_btn_generar): el comprobante de retención toma el siguiente número del correlativo de la
    /// planta (MPlantas.numero_ret). En el repo ese correlativo es Planta.NumeroRet con formato "000-00000".
    /// El legacy usaba siempre la planta 'OF'; aquí se usa la planta del pago.
    /// </summary>
    public static class ComprobanteRetencionNumerador
    {
        public static string Next(string actual)
        {
            var texto = (actual ?? string.Empty).Trim();
            var guion = texto.IndexOf('-');

            if (guion > 0)
            {
                var prefijo = texto[..guion];
                var numero = texto[(guion + 1)..];
                if (!long.TryParse(numero, out var n))
                {
                    throw new InvalidOperationException($"El correlativo de retención '{texto}' no es válido.");
                }

                return prefijo + "-" + (n + 1).ToString().PadLeft(numero.Length, '0');
            }

            // Formato legacy: 4 caracteres de prefijo + 6 dígitos.
            if (texto.Length != 10 || !long.TryParse(texto[4..], out var legacy))
            {
                throw new InvalidOperationException($"El correlativo de retención '{texto}' no es válido.");
            }

            return texto[..4] + (legacy + 1).ToString().PadLeft(6, '0');
        }

        /// <summary>Toma el siguiente número de la planta, con bloqueo, y deja la planta actualizada. Dentro de una transacción.</summary>
        public static async Task<string> TomarAsync(IUnitOfWork uow, string plantaCode, CancellationToken cancellationToken)
        {
            await uow.AcquireAppLockAsync($"RETENCION:{plantaCode}", 15000, cancellationToken);

            var planta = await uow.Comunes.Plantas.Query()
                .FirstOrDefaultAsync(p => p.Code == plantaCode, cancellationToken)
                ?? throw new InvalidOperationException($"Código {plantaCode} de Planta no registrado....");

            var siguiente = Next(planta.NumeroRet);
            planta.NumeroRet = siguiente;
            return siguiente;
        }
    }
}
