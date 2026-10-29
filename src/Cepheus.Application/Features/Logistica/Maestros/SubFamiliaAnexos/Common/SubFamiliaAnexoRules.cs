using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;

/// <summary>Reglas de validación compartidas por Create y Update (cuenta, tipo de anexo y anexo).</summary>
internal static class SubFamiliaAnexoRules
{
    public static void AddAccountingRules<T>(
        AbstractValidator<T> validator,
        IUnitOfWork uow,
        Func<T, string?> account,
        Func<T, string?> attachmentType,
        Func<T, string?> attachment)
    {
        validator.RuleFor(x => account(x))
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("La cuenta contable es obligatoria.")
            .MaximumLength(8).WithMessage("La cuenta contable no puede exceder los 8 caracteres.")
            .MustAsync((code, ct) => uow.Contabilidad.Maestros.CuentasContables.Query()
                .AnyAsync(c => c.Code == code!.Trim(), ct))
            .WithMessage("La cuenta contable indicada no existe.");

        validator.RuleFor(x => attachmentType(x))
            .MaximumLength(3).WithMessage("El tipo de anexo no puede exceder los 3 caracteres.")
            .MustAsync((code, ct) => uow.Contabilidad.Catalogos.TiposAnexo.Query()
                .AnyAsync(t => t.Code == code!.Trim(), ct))
            .WithMessage("El tipo de anexo indicado no existe.")
            .When(x => !string.IsNullOrWhiteSpace(attachmentType(x)));

        validator.RuleFor(x => attachment(x))
            .MaximumLength(5).WithMessage("El anexo no puede exceder los 5 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(attachment(x)));

        validator.RuleFor(x => attachment(x))
            .Must((x, _) => !string.IsNullOrWhiteSpace(attachmentType(x)))
            .WithMessage("Para indicar un anexo debe indicar también su tipo de anexo.")
            .When(x => !string.IsNullOrWhiteSpace(attachment(x)));

        validator.RuleFor(x => attachment(x))
            .MustAsync(async (x, code, ct) => await uow.Contabilidad.Maestros.Anexos.Query()
                .AnyAsync(a => a.TipoAnexoCode == attachmentType(x)!.Trim() && a.Code == code!.Trim(), ct))
            .WithMessage("El anexo indicado no existe para ese tipo de anexo.")
            .When(x => !string.IsNullOrWhiteSpace(attachment(x)) && !string.IsNullOrWhiteSpace(attachmentType(x)));
    }
}
