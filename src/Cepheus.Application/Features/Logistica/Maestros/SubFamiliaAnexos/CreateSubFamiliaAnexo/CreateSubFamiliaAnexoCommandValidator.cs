using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.Common;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Maestros.SubFamiliaAnexos.CreateSubFamiliaAnexo;

public class CreateSubFamiliaAnexoCommandValidator : AbstractValidator<CreateSubFamiliaAnexoCommand>
{
    private readonly IUnitOfWork _uow;

    public CreateSubFamiliaAnexoCommandValidator(IUnitOfWork uow)
    {
        _uow = uow;

        RuleFor(x => x.SubCentroCostoCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El subcentro de costo es obligatorio.")
            .MaximumLength(6).WithMessage("El subcentro de costo no puede exceder los 6 caracteres.")
            .MustAsync(SubCentroExists).WithMessage("El subcentro de costo indicado no existe.");

        RuleFor(x => x.SubFamiliaCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("La subfamilia es obligatoria.")
            .MaximumLength(4).WithMessage("La subfamilia no puede exceder los 4 caracteres.")
            .MustAsync(SubFamiliaExists).WithMessage("La subfamilia indicada no existe.");

        RuleFor(x => x)
            .MustAsync(BeUnique)
            .WithMessage("Ya existe un enlace para ese subcentro de costo y esa subfamilia.")
            .When(x => !string.IsNullOrWhiteSpace(x.SubCentroCostoCode) && !string.IsNullOrWhiteSpace(x.SubFamiliaCode));

        SubFamiliaAnexoRules.AddAccountingRules(
            this, uow, x => x.AccountingAccountCode, x => x.AccountingAttachmentTypeCode, x => x.AccountingAttachmentCode);
    }

    private async Task<bool> SubCentroExists(string code, CancellationToken ct)
        => await _uow.Logistica.Maestros.SubCentrosCosto.Query().AnyAsync(s => s.Code == code.Trim().ToUpper(), ct);

    private async Task<bool> SubFamiliaExists(string code, CancellationToken ct)
        => await _uow.Logistica.Catalogos.SubFamilias.Query().AnyAsync(s => s.Code == code.Trim().ToUpper(), ct);

    private async Task<bool> BeUnique(CreateSubFamiliaAnexoCommand command, CancellationToken ct)
    {
        var scc = command.SubCentroCostoCode.Trim().ToUpper();
        var sfa = command.SubFamiliaCode.Trim().ToUpper();
        return !await _uow.Logistica.Maestros.SubFamiliaAnexos.Query()
            .AnyAsync(x => x.SubCentroCostoCode == scc && x.SubFamiliaCode == sfa, ct);
    }
}
