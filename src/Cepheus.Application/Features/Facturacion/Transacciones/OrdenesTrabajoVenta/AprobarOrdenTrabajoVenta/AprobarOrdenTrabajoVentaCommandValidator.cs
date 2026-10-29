using Cepheus.Domain.Facturacion.Enum;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.AprobarOrdenTrabajoVenta;

public sealed class AprobarOrdenTrabajoVentaCommandValidator : AbstractValidator<AprobarOrdenTrabajoVentaCommand>
{
    public AprobarOrdenTrabajoVentaCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(8);
        RuleFor(x => x.Data).NotNull();

        When(x => x.Data is not null, () =>
        {
            RuleFor(x => x.Data.CreditStatus).IsInEnum();
            RuleFor(x => x.Data.PaymentType).IsInEnum();
            RuleFor(x => x.Data.DocumentType).IsInEnum().When(x => x.Data.DocumentType.HasValue);
            RuleFor(x => x.Data.DocumentFlow).IsInEnum().When(x => x.Data.DocumentFlow.HasValue);
            RuleFor(x => x.Data.ValorizationCode).MaximumLength(1);
            RuleFor(x => x.Data.ValorizationWeekday).IsInEnum().When(x => x.Data.ValorizationWeekday.HasValue);
            RuleFor(x => x.Data.ValorizationDays).GreaterThan(0).When(x => x.Data.ValorizationDays.HasValue);
            RuleFor(x => x.Data.FormaPagoVentaCode).MaximumLength(2);
            RuleFor(x => x.Data.Plant1Code).MaximumLength(2);
            RuleFor(x => x.Data.Plant2Code).MaximumLength(2);
            RuleFor(x => x.Data.Document1Number).MaximumLength(10);
            RuleFor(x => x.Data.Document2Number).MaximumLength(10);
            RuleFor(x => x.Data.GuideMotivoCode).MaximumLength(2);
            RuleFor(x => x.Data.PaymentMethodCode).MaximumLength(2);
            RuleFor(x => x.Data.BankCode).MaximumLength(10);
            RuleFor(x => x.Data.PaymentDocumentNumber).MaximumLength(50);
            RuleFor(x => x.Data.CreditsObservations).MaximumLength(4000);
            RuleFor(x => x.Data.ApprovalObservations).MaximumLength(4000);
            RuleFor(x => x.Data.DelayReason).MaximumLength(500);
            RuleFor(x => x.Data.ProductionPassReason).MaximumLength(500);

            var aprobado = (Func<AprobarOrdenTrabajoVentaCommand, bool>)(x => x.Data.CreditStatus == EstadoCreditoAprobacion.Aprobado);
            var contado = (Func<AprobarOrdenTrabajoVentaCommand, bool>)(x => aprobado(x) && x.Data.PaymentType == TipoPagoAprobacion.Contado);

            RuleFor(x => x.Data.Plant1Code).NotEmpty().When(contado)
                .WithMessage("En Contado debe indicar la planta del documento (Plant1Code).");

            RuleFor(x => x.Data.Plant2Code).NotEmpty().When(x => contado(x) && x.Data.HasTwoDocuments)
                .WithMessage("Con doble documento debe indicar la planta del segundo documento (Plant2Code).");

            RuleFor(x => x.Data.PaymentMethodCode).NotEmpty().When(contado)
                .WithMessage("En Contado debe indicar la forma de cobro (PaymentMethodCode).");

            RuleFor(x => x.Data.BankCode).NotEmpty()
                .When(x => contado(x) && !string.IsNullOrWhiteSpace(x.Data.PaymentMethodCode)
                           && x.Data.PaymentMethodCode!.Trim() is not ("01" or "04" or "10"))
                .WithMessage("La forma de cobro indicada exige banco.");

            RuleFor(x => x.Data.PaymentDocumentNumber).NotEmpty()
                .When(x => contado(x) && !string.IsNullOrWhiteSpace(x.Data.PaymentMethodCode)
                           && x.Data.PaymentMethodCode!.Trim() != "01")
                .WithMessage("La forma de cobro indicada exige el N° de pago.");

            RuleFor(x => x.Data)
                .Must(d => d.PaymentType == TipoPagoAprobacion.Contado
                           || (string.IsNullOrWhiteSpace(d.Document1Number)
                               && string.IsNullOrWhiteSpace(d.Document2Number)
                               && !d.HasTwoDocuments
                               && (d.DocumentFlow is null || d.DocumentFlow == FlujoDocumentoAprobacion.SinDocumento)))
                .WithMessage("Una aprobación a Crédito no lleva documentos de pago.");

            RuleFor(x => x.Data)
                .Must(d => d.PaymentType != TipoPagoAprobacion.Contado
                           || d.DocumentFlow != FlujoDocumentoAprobacion.SinDocumento)
                .When(x => x.Data.CreditStatus == EstadoCreditoAprobacion.Aprobado)
                .WithMessage("Una aprobación al Contado exige tipo de comprobante (TipoP, TipoG o TipoC).");

            RuleFor(x => x.Data.ProductionPassReason).NotEmpty()
                .When(x => aprobado(x) && x.Data.PassToProduction == false)
                .WithMessage("Si no pasa a producción debe indicar el motivo.");
        });
    }
}
