using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Contabilidad.Enum;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Maestros.PlantillasAsiento.Common
{
    public abstract class PlantillaAsientoValidator<T> : AbstractValidator<T>
        where T : IPlantillaAsientoCabecera
    {
        private static readonly ConceptoAsiento[] ConceptosProvision =
        {
            ConceptoAsiento.TotalDocumento, ConceptoAsiento.DetalleDocumento, ConceptoAsiento.Igv,
            ConceptoAsiento.IgvExterior, ConceptoAsiento.Renta, ConceptoAsiento.Fonavi,
            ConceptoAsiento.NoGravable, ConceptoAsiento.Servicio
        };

        private static readonly ConceptoAsiento[] ConceptosReversionCajaChica =
{
            ConceptoAsiento.TotalDocumento
        };

        private static readonly ConceptoAsiento[] ConceptosTransferencia =
        {
            ConceptoAsiento.MontoNotaIngreso, ConceptoAsiento.NetoNotaIngreso
        };

        private static readonly ConceptoAsiento[] ConceptosConsumo =
        {
            ConceptoAsiento.DetalleVale, ConceptoAsiento.TotalVales
        };

        private static bool EsProvision(ProcesoAsiento p) =>
            p == ProcesoAsiento.ProvisionCompraNotaIngreso || p == ProcesoAsiento.ProvisionOrdenServicio
            || p == ProcesoAsiento.ProvisionCajaChica;

        private static bool EsTransferencia(ProcesoAsiento p) =>
            p == ProcesoAsiento.TransferenciaNotaIngreso || p == ProcesoAsiento.TransferenciaOrdenServicio;

        private static ConceptoAsiento[] ConceptosPermitidos(ProcesoAsiento p) =>
            p == ProcesoAsiento.ReversionCajaChica ? ConceptosReversionCajaChica
            : EsProvision(p) ? ConceptosProvision : EsTransferencia(p) ? ConceptosTransferencia : ConceptosConsumo;

        protected PlantillaAsientoValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Process)
                .IsInEnum().WithMessage("El proceso no es válido.");

            RuleFor(x => x.TipoAsientoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tipo de asiento es obligatorio.")
                .MustAsync(async (c, ct) =>
                    await uow.Contabilidad.Catalogos.TiposAsiento.Query()
                        .AnyAsync(t => t.Code == c.Trim().ToUpper() && t.IsActive, ct))
                .WithMessage("El tipo de asiento no existe o está inactivo.");

            // Cuenta de compras: solo en la provisión de compras por nota de ingreso.
            RuleFor(x => x.PurchaseAccountCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La cuenta de compras es obligatoria para la provisión de compras.")
                .MustAsync(async (c, ct) => await AccountExists(uow, c!, ct))
                .WithMessage("La cuenta de compras no existe o está inactiva.")
                .When(x => x.Process == ProcesoAsiento.ProvisionCompraNotaIngreso);

            RuleFor(x => x.PurchaseAccountCode)
                .Must(c => string.IsNullOrWhiteSpace(c))
                .WithMessage("La cuenta de compras solo aplica a la provisión de compras de la nota de ingreso.")
                .When(x => x.Process != ProcesoAsiento.ProvisionCompraNotaIngreso);

            // Consumo: comprobante y texto fijo obligatorios.
            RuleFor(x => x.ComprobantePagoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El comprobante de pago es obligatorio para el consumo de vales.")
                .MustAsync(async (c, ct) =>
                    await uow.Comunes.ComprobantesPago.Query()
                        .AnyAsync(p => p.Code == c!.Trim().ToUpper() && p.IsActive, ct))
                .WithMessage("El comprobante de pago no existe o está inactivo.")
                .When(x => x.Process == ProcesoAsiento.ConsumoVales);

            RuleFor(x => x.ComprobantePagoCode)
                .Must(c => string.IsNullOrWhiteSpace(c))
                .WithMessage("El comprobante de pago de la plantilla solo aplica al consumo de vales.")
                .When(x => x.Process != ProcesoAsiento.ConsumoVales);

            RuleFor(x => x.DescriptionText)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El texto de la descripción es obligatorio para el consumo de vales.")
                .MaximumLength(500)
                .When(x => x.Process == ProcesoAsiento.ConsumoVales);

            RuleFor(x => x.DescriptionText)
                .MaximumLength(500)
                .When(x => x.Process != ProcesoAsiento.ConsumoVales);

            RuleFor(x => x.Lines)
                .NotEmpty().WithMessage("La plantilla debe tener al menos una línea.");

            RuleFor(x => x.Lines)
                .Custom((lines, context) => ValidateLines(context.InstanceToValidate, lines, context))
                .When(x => x.Lines is { Count: > 0 });

            RuleFor(x => x.Lines)
                .MustAsync(async (lines, ct) =>
                {
                    var codes = lines
                        .Where(l => l.AccountSource == OrigenCuentaAsiento.Fija && !string.IsNullOrWhiteSpace(l.AccountCode))
                        .Select(l => l.AccountCode!.Trim().ToUpper())
                        .Distinct()
                        .ToList();

                    if (codes.Count == 0)
                    {
                        return true;
                    }

                    var found = await uow.Contabilidad.Maestros.CuentasContables.Query()
                        .Where(c => codes.Contains(c.Code) && c.IsActive)
                        .CountAsync(ct);

                    return found == codes.Count;
                })
                .WithMessage("Alguna cuenta fija de las líneas no existe o está inactiva.")
                .When(x => x.Lines is { Count: > 0 });

            RuleFor(x => x.Lines)
                .MustAsync(async (lines, ct) =>
                {
                    var codes = lines
                        .Where(l => !string.IsNullOrWhiteSpace(l.ValeTypeCode))
                        .Select(l => l.ValeTypeCode!.Trim().ToUpper())
                        .Distinct()
                        .ToList();

                    if (codes.Count == 0)
                    {
                        return true;
                    }

                    var found = await uow.Logistica.Catalogos.TiposVale.Query()
                        .Where(t => codes.Contains(t.Code))
                        .CountAsync(ct);

                    return found == codes.Count;
                })
                .WithMessage("Algún tipo de vale de las líneas no existe.")
                .When(x => x.Lines is { Count: > 0 });
        }

        private static async Task<bool> AccountExists(IUnitOfWork uow, string code, CancellationToken ct)
            => await uow.Contabilidad.Maestros.CuentasContables.Query()
                .AnyAsync(c => c.Code == code.Trim().ToUpper() && c.IsActive, ct);

        private static void ValidateLines(
            IPlantillaAsientoCabecera plantilla,
            List<PlantillaAsientoLineaInput> lines,
            ValidationContext<T> context)
        {
            var process = plantilla.Process;
            var allowed = ConceptosPermitidos(process);

            if (lines.Select(l => l.Order).Distinct().Count() != lines.Count)
            {
                context.AddFailure("Lines", "El orden de las líneas no puede repetirse.");
            }

            foreach (var line in lines)
            {
                var label = $"Línea {line.Order}";

                if (line.Order <= 0)
                {
                    context.AddFailure("Lines", $"{label}: el orden debe ser mayor a 0.");
                }

                if (!System.Enum.IsDefined(line.Concept) || !System.Enum.IsDefined(line.Nature)
                    || !System.Enum.IsDefined(line.AccountSource) || !System.Enum.IsDefined(line.AnnexSource)
                    || !System.Enum.IsDefined(line.DescriptionSource) || !System.Enum.IsDefined(line.AccountingType))
                {
                    context.AddFailure("Lines", $"{label}: algún valor de enumeración no es válido.");
                    continue;
                }

                if (!allowed.Contains(line.Concept))
                {
                    context.AddFailure("Lines", $"{label}: el concepto {line.Concept} no aplica al proceso {process}.");
                }

                if (line.AccountSource == OrigenCuentaAsiento.Fija)
                {
                    if (string.IsNullOrWhiteSpace(line.AccountCode))
                    {
                        context.AddFailure("Lines", $"{label}: la cuenta es obligatoria cuando el origen de la cuenta es Fija.");
                    }
                }
                else if (!string.IsNullOrWhiteSpace(line.AccountCode))
                {
                    context.AddFailure("Lines", $"{label}: solo se indica cuenta cuando el origen de la cuenta es Fija.");
                }

                if (line.AccountSource == OrigenCuentaAsiento.LineaDocumento && line.Concept != ConceptoAsiento.DetalleDocumento)
                {
                    context.AddFailure("Lines", $"{label}: la cuenta de la línea del documento solo aplica al concepto DetalleDocumento.");
                }

                if (line.AccountSource == OrigenCuentaAsiento.CuentaPorPagarProveedor && line.Concept != ConceptoAsiento.TotalDocumento)
                {
                    context.AddFailure("Lines", $"{label}: la cuenta por pagar del proveedor solo aplica al concepto TotalDocumento.");
                }

                if (line.Concept == ConceptoAsiento.DetalleDocumento && line.AccountSource == OrigenCuentaAsiento.CuentaPorPagarProveedor)
                {
                    context.AddFailure("Lines", $"{label}: DetalleDocumento no puede usar la cuenta por pagar del proveedor.");
                }

                if (line.AccountSource == OrigenCuentaAsiento.SubCentroCosto && line.Concept != ConceptoAsiento.DetalleVale)
                {
                    context.AddFailure("Lines", $"{label}: la cuenta del subcentro de costo solo aplica al concepto DetalleVale.");
                }

                if (line.AnnexSource == OrigenAnexoAsiento.LineaDocumento && line.Concept != ConceptoAsiento.DetalleDocumento)
                {
                    context.AddFailure("Lines", $"{label}: el anexo de la línea del documento solo aplica al concepto DetalleDocumento.");
                }

                if (line.AnnexSource == OrigenAnexoAsiento.Proveedor && !EsProvision(process))
                {
                    context.AddFailure("Lines", $"{label}: el anexo del proveedor solo aplica a la provisión de compras o de orden de servicio.");
                }

                if (line.AnnexSource == OrigenAnexoAsiento.ArticuloSubCentro && line.Concept != ConceptoAsiento.DetalleVale)
                {
                    context.AddFailure("Lines", $"{label}: el anexo por artículo y subcentro solo aplica al concepto DetalleVale.");
                }

                if (!string.IsNullOrWhiteSpace(line.ValeTypeCode) && line.Concept != ConceptoAsiento.DetalleVale)
                {
                    context.AddFailure("Lines", $"{label}: el tipo de vale solo aplica al concepto DetalleVale.");
                }

                if (line.DescriptionSource == OrigenDescripcionAsiento.TextoPlantilla
                    && string.IsNullOrWhiteSpace(plantilla.DescriptionText))
                {
                    context.AddFailure("Lines", $"{label}: usa el texto de la plantilla pero la plantilla no define el texto de la descripción.");
                }
            }

            if (!lines.Any(l => l.Nature == NaturalezaAsiento.Debe) || !lines.Any(l => l.Nature == NaturalezaAsiento.Haber))
            {
                context.AddFailure("Lines", "La plantilla debe tener al menos una línea al Debe y una al Haber.");
            }

            if (process == ProcesoAsiento.ConsumoVales)
            {
                var detalle = lines.Where(l => l.Concept == ConceptoAsiento.DetalleVale).ToList();

                if (detalle.Count == 0)
                {
                    context.AddFailure("Lines", "El consumo de vales necesita al menos una línea de concepto DetalleVale.");
                }

                if (detalle.Count(l => string.IsNullOrWhiteSpace(l.ValeTypeCode)) != 1)
                {
                    context.AddFailure("Lines", "Debe haber exactamente una línea DetalleVale general (sin tipo de vale).");
                }

                var tipos = detalle
                    .Where(l => !string.IsNullOrWhiteSpace(l.ValeTypeCode))
                    .Select(l => l.ValeTypeCode!.Trim().ToUpperInvariant())
                    .ToList();

                if (tipos.Distinct().Count() != tipos.Count)
                {
                    context.AddFailure("Lines", "No puede haber dos líneas DetalleVale para el mismo tipo de vale.");
                }

                if (lines.Count(l => l.Concept == ConceptoAsiento.TotalVales) != 1)
                {
                    context.AddFailure("Lines", "El consumo de vales necesita exactamente una línea de concepto TotalVales.");
                }

                if (detalle.Any(l => l.AccountSource != OrigenCuentaAsiento.SubCentroCosto && l.AccountSource != OrigenCuentaAsiento.Fija))
                {
                    context.AddFailure("Lines", "Las líneas DetalleVale usan la cuenta del subcentro de costo o una cuenta fija.");
                }

                if (lines.Any(l => l.Concept == ConceptoAsiento.TotalVales && l.AccountSource != OrigenCuentaAsiento.Fija))
                {
                    context.AddFailure("Lines", "La línea TotalVales usa una cuenta fija.");
                }
            }
        }
    }
}
