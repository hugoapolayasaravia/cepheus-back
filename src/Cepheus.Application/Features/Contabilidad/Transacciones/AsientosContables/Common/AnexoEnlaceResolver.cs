using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Maestros;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common
{
    /// <summary>
    /// Resuelve la cuenta contable y el anexo con que se imputa un artículo en un subcentro de costo
    /// (legacy: validación de la orden de servicio y del consumo):
    ///   - La CUENTA es la del subcentro de costo (SubCentroCosto.AccountingAccountCode; debe tener 8 caracteres y
    ///     existir en CuentaContable).
    ///   - Si la cuenta exige tipo de anexo (3 caracteres), el ANEXO sale de ArticuloAnexo (subcentro + artículo) y,
    ///     si no hay, de SubFamiliaAnexo (subcentro + subfamilia = 4 primeros caracteres del artículo). Si no hay
    ///     ninguno (anexo de 5 caracteres), es un error.
    ///   - Si la cuenta no exige tipo de anexo, no lleva anexo.
    /// La cuenta y el anexo propios de ArticuloAnexo / SubFamiliaAnexo no se usan: igual que el legacy, de esas tablas
    /// solo se toma el código del anexo.
    /// </summary>
    public static class AnexoEnlaceResolver
    {
        public sealed record Resultado(string CuentaCode, string? AnexoTipoCode, string? AnexoCode, CuentaContable Cuenta);

        public static async Task<Resultado> ResolverAsync(
            IUnitOfWork uow,
            string subCentroCostoCode,
            string? cuentaDelSubCentro,
            string articuloCode,
            CancellationToken cancellationToken)
        {
            var scc = subCentroCostoCode.Trim();
            var cta = (cuentaDelSubCentro ?? string.Empty).Trim();

            if (cta.Length != 8)
            {
                throw new InvalidOperationException(
                    $"Sub centro de costo {scc} no tiene cuenta contable ... Verifique ...");
            }

            var cuenta = await uow.Contabilidad.Maestros.CuentasContables.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Code == cta, cancellationToken)
                ?? throw new InvalidOperationException(
                    $"Cuenta {cta} del sub centro de costo {scc} no registrada ... Verifique Tipo de Anexo ...");

            var tipo = (cuenta.TipoAnexoCode ?? string.Empty).Trim();
            if (tipo.Length != 3)
            {
                return new Resultado(cta, null, null, cuenta);
            }

            var art = articuloCode.Trim();

            var anexo = await uow.Logistica.Maestros.ArticuloAnexos.Query()
                .AsNoTracking()
                .Where(a => a.SubCentroCostoCode == scc && a.ArticuloCode == art)
                .Select(a => a.AccountingAttachmentCode)
                .FirstOrDefaultAsync(cancellationToken);

            if ((anexo ?? string.Empty).Trim().Length != 5)
            {
                var subFamilia = art.Length >= 4 ? art.Substring(0, 4) : art;

                anexo = await uow.Logistica.Maestros.SubFamiliaAnexos.Query()
                    .AsNoTracking()
                    .Where(a => a.SubCentroCostoCode == scc && a.SubFamiliaCode == subFamilia)
                    .Select(a => a.AccountingAttachmentCode)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            if ((anexo ?? string.Empty).Trim().Length != 5)
            {
                throw new InvalidOperationException(
                    $"El artículo {art} no tiene anexo contable en el sub centro de costo {scc} " +
                    "(ni por artículo ni por subfamilia) ... Verifique Tipo de Anexo ...");
            }

            return new Resultado(cta, tipo, anexo!.Trim(), cuenta);
        }
    }
}
