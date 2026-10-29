using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloAnexos.Common
{
    public abstract class ArticuloAnexoValidator<T> : AbstractValidator<T>
        where T : IArticuloAnexoDatos
    {
        protected ArticuloAnexoValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.SubCentroCostoCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El subcentro de costo es obligatorio.")
                .MaximumLength(6)
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Maestros.SubCentrosCosto.Query()
                        .AnyAsync(s => s.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El subcentro de costo no existe.");

            RuleFor(x => x.ArticuloCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El artículo es obligatorio.")
                .MaximumLength(7)
                .MustAsync(async (c, ct) =>
                    await uow.Logistica.Maestros.Articulos.Query()
                        .AnyAsync(a => a.Code == c.Trim().ToUpper(), ct))
                .WithMessage("El artículo no existe.");

            RuleFor(x => x.AccountingAccountCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("La cuenta contable es obligatoria.")
                .MaximumLength(8)
                .MustAsync(async (c, ct) =>
                    await uow.Contabilidad.Maestros.CuentasContables.Query()
                        .AnyAsync(a => a.Code == c.Trim().ToUpper() && a.IsActive, ct))
                .WithMessage("La cuenta contable no existe o está inactiva.");

            RuleFor(x => x)
                .Must(x => string.IsNullOrWhiteSpace(x.AccountingAttachmentTypeCode)
                           == string.IsNullOrWhiteSpace(x.AccountingAttachmentCode))
                .WithMessage("El anexo requiere tipo de anexo y código.")
                .OverridePropertyName("AccountingAttachment");

            RuleFor(x => x)
                .MustAsync(async (x, ct) =>
                {
                    var tipo = x.AccountingAttachmentTypeCode!.Trim().ToUpper();
                    var code = x.AccountingAttachmentCode!.Trim().ToUpper();
                    return await uow.Contabilidad.Maestros.Anexos.Query()
                        .AnyAsync(a => a.TipoAnexoCode == tipo && a.Code == code, ct);
                })
                .WithMessage("El anexo indicado no existe.")
                .OverridePropertyName("AccountingAttachment")
                .When(x => !string.IsNullOrWhiteSpace(x.AccountingAttachmentTypeCode)
                           && !string.IsNullOrWhiteSpace(x.AccountingAttachmentCode));
        }
    }
}
